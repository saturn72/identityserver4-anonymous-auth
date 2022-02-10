using IdentityModel;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using IdentityServer4.Anonnymous.Events;
using IdentityServer4.Anonnymous.ResponseHandling;
using System;
using IdentityServer4.Anonnymous.Endpoints.Results;

namespace IdentityServer4.Anonnymous.Endpoints
{
    public class AnonnymousAuthorizationEndpointHandler : IEndpointHandler
    {
        private readonly IClientSecretValidator _clientValidator;
        private readonly IAnonnymousAuthorizationRequestValidator _requestValidator;
        private readonly IAuthorizationResponseGenerator _responseGenerator;
        private readonly IEventService _events;
        private readonly ILogger<AnonnymousAuthorizationEndpointHandler> _logger;

        public AnonnymousAuthorizationEndpointHandler(
            IClientSecretValidator clientValidator,
            IAnonnymousAuthorizationRequestValidator requestValidator,
            IAuthorizationResponseGenerator responseGenerator,
            IEventService events,
            ILogger<AnonnymousAuthorizationEndpointHandler> logger)
        {
            _clientValidator = clientValidator;
            _requestValidator = requestValidator;
            _responseGenerator = responseGenerator;
            _events = events;
            _logger = logger;
        }
        public async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            _logger.LogTrace("Processing phone authorize request.");

            // validate HTTP
            if (!HttpMethods.IsPost(context.Request.Method) ||
                !context.Request.HasApplicationFormContentType())
            {
                _logger.LogWarning("Invalid HTTP request for phone authorize endpoint");
                return Error(OidcConstants.TokenErrors.InvalidRequest);
            }

            return await ProcessAuthorizationRequestAsync(context);
        }

        private TokenErrorResult Error(string error, string errorDescription = null, Dictionary<string, object> custom = null)
        {
            var response = new TokenErrorResponse
            {
                Error = error,
                ErrorDescription = errorDescription,
                Custom = custom
            };

            _logger.LogError("Phone authorization error: {error}:{errorDescriptions}", error, error ?? "-no message-");

            return new TokenErrorResult(response);
        }
        private async Task<IEndpointResult> ProcessAuthorizationRequestAsync(HttpContext context)
        {
            _logger.LogDebug("Start phone authorize request.");

            // validate client
            var clientResult = await _clientValidator.ValidateAsync(context);
            if (clientResult.Client == null) return Error(OidcConstants.TokenErrors.InvalidClient);

            // validate request
            var form = (await context.Request.ReadFormAsync()).AsNameValueCollection();
            var requestResult = await _requestValidator.ValidateAsync(form, clientResult);

            if (requestResult.IsError)
            {
                await _events.RaiseAsync(new AnonnymousAuthorizationFailureEvent(requestResult));
                return Error(requestResult.Error, requestResult.ErrorDescription);
            }

            var baseUrl = context.GetIdentityServerBaseUrl().EnsureTrailingSlash();

            // create response
            _logger.LogTrace("Calling into phone authorize response generator: {type}", _responseGenerator.GetType().FullName);
            var response = await _responseGenerator.ProcessAsync(requestResult, baseUrl);

            await _events.RaiseAsync(new AnonnymousAuthorizationSuccessEvent(response, requestResult));

            // return result
            _logger.LogDebug("Phone authorize request success.");
            return new AuthorizationResult(response);
        }
    }
}