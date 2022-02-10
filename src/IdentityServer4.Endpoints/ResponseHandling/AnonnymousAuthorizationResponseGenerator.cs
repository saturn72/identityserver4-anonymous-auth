using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Anonnymous.Services;
using IdentityServer4.Anonnymous.Validation;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IdentityServer4.Anonnymous.ResponseHandling
{
    public class AnonnymousAuthorizationResponseGenerator : IAuthorizationResponseGenerator
    {
        #region fields
        private readonly IUserCodeService _userCodeService;
        private readonly AnonnymousAuthorizationOptions _options;
        private readonly IAnonnymousCodeService _anonnymousCodeService;
        private readonly IEnumerable<ITransport> _transports;
        private readonly ISystemClock _clock;
        private readonly IHandleGenerationService _handleGenerationService;
        private readonly ILogger<AnonnymousAuthorizationResponseGenerator> _logger;
        #endregion

        #region ctor
        public AnonnymousAuthorizationResponseGenerator(
            IUserCodeService userCodeService,
            IOptions<AnonnymousAuthorizationOptions> options,
            IAnonnymousCodeService anonnymousCodeService,
            IEnumerable<ITransport> transports,
            ISystemClock clock,
            IHandleGenerationService handleGenerationService,
            ILogger<AnonnymousAuthorizationResponseGenerator> logger)
        {
            _userCodeService = userCodeService;
            _options = options.Value;
            _anonnymousCodeService = anonnymousCodeService;
            _transports = transports;
            _clock = clock;
            _handleGenerationService = handleGenerationService;
            _logger = logger;

        }
        #endregion
        public async Task<AuthorizationResponse> ProcessAsync(AuthorizationRequestValidationResult validationResult, string baseUrl)
        {
            _logger.LogInformation("start Processing anonnymous request");
            if (validationResult == null) throw new ArgumentNullException(nameof(validationResult));
            if (!baseUrl.HasValue()) throw new ArgumentException("Value cannot be null or whitespace.", nameof(baseUrl));

            var validatedRequest = validationResult.ValidatedRequest;
            var client = validatedRequest?.Client;
            if (client == null) throw new ArgumentNullException(nameof(validationResult.ValidatedRequest.Client));

            _logger.LogDebug("Creating response for phone authorization request");
            var response = new AuthorizationResponse();

            var verificationCode = await _handleGenerationService.GenerateAsync();
            _logger.LogDebug($"anonnymous-code was generated valued: {verificationCode}");
            response.VerificationCode = verificationCode;
            // generate activation URIs
            response.VerificationUri = _options.VerificationUri;
            response.VerificationUriComplete = $"{_options.VerificationUri.RemoveTrailingSlash()}?{Constants.UserInteraction.VerificationCode}={response.VerificationCode}";

            // lifetime
            response.Lifetime = client.TryGetIntPropertyOrDefault(_options.LifetimePropertyName, _options.DefaultCodeLifetime);
            _logger.LogDebug($"phone lifetime was set to {response.Lifetime}");

            // interval
            response.Interval = _options.Interval;
            _logger.LogDebug($"phone interval was set to {response.Interval}");

            //allowed retries
            var allowedRetries = client.TryGetIntPropertyOrDefault(_options.AllowedRetriesPropertyName, _options.AllowedRetries);
            _logger.LogDebug($"Max allowed retries was set to {allowedRetries}");

            var userCode = await GenerateUserCodeAsync(client.UserCodeType ?? _options.DefaultUserCodeType);
            var ac = new AnonnymousCodeInfo
            {
                AllowedRetries = allowedRetries,
                ClientId = client.ClientId,
                CreatedOnUtc = _clock.UtcNow.UtcDateTime,
                Description = validatedRequest.Description,
                Lifetime = response.Lifetime,
                ReturnUrl = validatedRequest.RedirectUrl,
                RequestedScopes = validatedRequest.RequestedScopes,
                UserCode = userCode.Sha256(),
                Transport = validatedRequest.Transport,
                VerificationCode = response.VerificationCode,
            };
            _logger.LogDebug($"storing anonnymous-code in database: {ac.ToJsonString()}");
            _ = _anonnymousCodeService.StoreAnonnymousCodeInfoAsync(response.VerificationCode, ac);

            _logger.LogDebug("Send code via transports");
            var codeContext = new UserCodeTransportContext
            {
                Transport = validatedRequest.Transport,
                Data = validatedRequest.TransportData,
                Provider = validatedRequest.Provider,
            };
            codeContext.Body = await BuildMessageBody(client, userCode, codeContext);
            _ = _transports.Transport(codeContext);
            return response;
        }
        private Task<string> BuildMessageBody(Client client, string code, UserCodeTransportContext ctx)
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
                var stored = await _anonnymousCodeService.FindByUserCodeAsync(userCode);
                if (stored == null)
                    return userCode;
                retryCount++;
            }
            throw new InvalidOperationException("Unable to create unique user-code for anonnymous flow");
        }
    }

}