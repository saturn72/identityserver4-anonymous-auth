using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using IdentityServer4.Endpoints.Results;
using System.Net;
using IdentityServer4.Extensions;
using IdentityServer4.Anonnymous.Services;
using IdentityServer4.ResponseHandling;
using System.Collections.Generic;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Options;
using IdentityServer4.Anonnymous.Endpoints.Results;
using System;
using IdentityServer4.Models;

namespace IdentityServer4.Anonnymous.Endpoints
{
    public class AnonnymousActivationEndpointHandler : IEndpointHandler
    {
        private readonly IAnonnymousCodeService _anonnymousCodeService;
        private readonly IUserCodeService _userCodeService;
        private readonly IClientStore _clientStore;
        private readonly AnonnymousAuthorizationOptions _options;
        private readonly IEnumerable<ITransport> _transports;
        private readonly ILogger<AnonnymousActivationEndpointHandler> _logger;

        public AnonnymousActivationEndpointHandler(
            IAnonnymousCodeService anonnymousCodeService,
            IUserCodeService userCodeService,
            IClientStore clientStore,
            IOptions<AnonnymousAuthorizationOptions> options,
            IEnumerable<ITransport> transports,
            ILogger<AnonnymousActivationEndpointHandler> logger)
        {
            _anonnymousCodeService = anonnymousCodeService;
            _userCodeService = userCodeService;
            _clientStore = clientStore;
            _options = options.Value;
            _transports = transports;
            _logger = logger;
        }
        public async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            if (!HttpMethods.IsGet(context.Request.Method))
            {
                _logger.LogWarning("Invalid HTTP method for endpoint.");
                return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }

            _logger.LogDebug($"Start {nameof(ProcessAsync)}");

            var query = context.Request.Query.AsNameValueCollection();
            var anonymousCode = query[Constants.IdentityModel.AnonnymousCode];
            var entry = await _anonnymousCodeService.FindByAnonnymousCodeAsync(anonymousCode);
            if (entry == default)
                return await HandleErrorResultAsync($"fail to locate wntry with anonnymous_code: {anonymousCode}", "bad or missing data");


            // generate anonnymous_code
            var client = await _clientStore.FindClientByIdAsync(entry.ClientId);
            if (client == default)
                return await HandleErrorResultAsync($"fail to locate client with id: {entry.ClientId}", "bad or missing data");

            var code = await GenerateUserCodeAsync(client.UserCodeType ?? _options.DefaultUserCodeType);
            _ = _anonnymousCodeService.Activate(entry.Id, code);
            var ctx = new AnonnymousCodeTransportContext
            {
                Transport = entry.Transport,
                Provider = entry.TransportProvider,
                Data = entry.TransportData,
            };
            ctx.Body = await BuildMessageBody(client, code, ctx);
            _ = _transports.Transport(ctx);

            return new StatusCodeResult(HttpStatusCode.OK);
        }

        private Task<string> BuildMessageBody(Client client, string code, AnonnymousCodeTransportContext ctx)
        {
            if (!client.Properties.TryGetValue($"formats:{ctx.Transport}", out var msgFormat))
            {
                msgFormat = ctx.Transport switch
                {
                    Constants.TransportTypes.Email =>
                        client.Properties.TryGetValue(_options.DefaultUserCodeEmailFormatPropertyName, out var v) ?
                            v :
                            _options.DefaultUserCodeEmailFormat,
                    Constants.TransportTypes.Sms =>
                        client.Properties.TryGetValue(_options.DefaultUserCodeSmSFormatPropertyName, out var v) ?
                            v :
                            _options.DefaultUserCodeSmsFormat,
                    _ => throw new InvalidOperationException($"Cannot find message format \'{ctx.Transport}\' for client \'{client.ClientName}\'"),
                };
            }

            var body = msgFormat.Replace(Constants.Formats.Fields.UserCode, code);
            return Task.FromResult(body);
        }

        private async Task<string> GenerateUserCodeAsync(string userCodeType)
        {
            var userCodeGenerator = await _userCodeService.GetGenerator(userCodeType);
            var retryCount = 0;

            while (retryCount < userCodeGenerator.RetryLimit)
            {
                var userCode = await userCodeGenerator.GenerateAsync();
                var storeMfaCode = await _anonnymousCodeService.FindByUserCodeAsync(userCode);
                if (storeMfaCode == null)
                    return userCode;
                retryCount++;
            }
            throw new InvalidOperationException("Unable to create unique user-code for anonnymous flow");
        }

        private Task<ActivationErrorResult> HandleErrorResultAsync(string logMessage, string errorDescription = null)
        {
            _logger.LogError(logMessage);

            var error = new TokenErrorResponse
            {
                Error = "Failed to verify",
                ErrorDescription = errorDescription,
            };
            return Task.FromResult(new ActivationErrorResult(error));
        }
    }
}