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
using IdentityServer4.Anonymous.Events;
using IdentityServer4.Anonymous.Endpoints.Results;
using IdentityServer4.Anonymous.Services.Generators;
using IdentityServer4.Anonymous.Validation;

namespace IdentityServer4.Anonymous.Endpoints
{
    public class AnonymousAuthorizationEndpoint : IEndpointHandler
    {
        private readonly IClientSecretValidator _clientValidator;
        private readonly IAnonymousAuthorizationRequestValidator _requestValidator;
        private readonly IAuthorizationResponseGenerator _responseGenerator;
        private readonly IEventService _events;
        private readonly ILogger<AnonymousAuthorizationEndpoint> _logger;

        public AnonymousAuthorizationEndpoint(
            IClientSecretValidator clientValidator,
            IAnonymousAuthorizationRequestValidator requestValidator,
            IAuthorizationResponseGenerator responseGenerator,
            IEventService events,
            ILogger<AnonymousAuthorizationEndpoint> logger)
        {
            _clientValidator = clientValidator;
            _requestValidator = requestValidator;
            _responseGenerator = responseGenerator;
            _events = events;
            _logger = logger;
        }
        public async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            _logger.LogTrace("Processing anonymous authorize request.");

            // validate HTTP
            if (!HttpMethods.IsPost(context.Request.Method) || !context.Request.HasApplicationFormContentType())
            {
                _logger.LogWarning("Invalid HTTP request for anonymous authorize endpoint");
                return Error(OidcConstants.TokenErrors.InvalidRequest);
            }

            return await ProcessAuthorizationRequestAsync(context);
        }
        private async Task<IEndpointResult> ProcessAuthorizationRequestAsync(HttpContext context)
        {
            _logger.LogDebug("Start anonymous authorize request.");

            // validate client
            var clientResult = await _clientValidator.ValidateAsync(context);
            if (clientResult?.Client == default || clientResult.IsError) return Error(OidcConstants.TokenErrors.InvalidClient);

            // validate request
            var form = (await context.Request.ReadFormAsync()).AsNameValueCollection();
            var requestResult = await _requestValidator.ValidateAsync(form, clientResult);

            if (requestResult == default || requestResult.IsError)
            {
                await _events.RaiseAsync(new AnonymousAuthorizationFailureEvent(requestResult));
                return Error(requestResult?.Error, requestResult?.ErrorDescription);
            }

            // create response
            _logger.LogTrace("Calling into anonymous authorize response generator: {type}", _responseGenerator.GetType().FullName);
            var response = await _responseGenerator.ProcessAsync(requestResult);

            await _events.RaiseAsync(new AnonymousAuthorizationSuccessEvent(response, requestResult));

            // return result
            _logger.LogDebug("Anonymous authorize request success.");
            return new AuthorizationResult(response);
        }
        private TokenErrorResult Error(string error, string errorDescription = null, Dictionary<string, object> custom = null)
        {
            var response = new TokenErrorResponse
            {
                Error = error,
                ErrorDescription = errorDescription,
                Custom = custom
            };

            _logger.LogError("Anonymous authorization error: {error}:{errorDescriptions}", error, error ?? "-no message-");

            return new TokenErrorResult(response);
        }
    }
}