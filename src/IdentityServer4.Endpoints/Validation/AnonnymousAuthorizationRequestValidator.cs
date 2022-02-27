using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Linq;
using IdentityServer4.Anonnymous.Models;
using IdentityModel;
using System.Collections.Generic;
using IdentityServer4.Anonnymous.Logging;
using System.Text.Json;

namespace IdentityServer4.Anonnymous.Validation
{
    public sealed class AnonnymousAuthorizationRequestValidator : IAnonnymousAuthorizationRequestValidator
    {
        #region fields
        private readonly AnonnymousAuthorizationOptions _options;
        private readonly IResourceValidator _resourceValidator;
        private readonly ILogger<AnonnymousAuthorizationRequestValidator> _logger;
        #endregion
        #region ctor
        public AnonnymousAuthorizationRequestValidator(
            IOptions<AnonnymousAuthorizationOptions> options,
            IResourceValidator resourceValidator,
            ILogger<AnonnymousAuthorizationRequestValidator> logger)
        {
            _options = options.Value;
            _resourceValidator = resourceValidator;
            _logger = logger;
        }
        #endregion
        public async Task<AuthorizationRequestValidationResult> ValidateAsync(
            NameValueCollection parameters,
            ClientSecretValidationResult clientValidationResult)
        {
            _logger.LogDebug("Start anonnymous authorization request validation");

            var request = new ValidatedAnonnymousAuthorizationRequest
            {
                Raw = parameters ?? throw new ArgumentNullException(nameof(parameters)),
            };

            var requestResult = ValidateRequestParameters(request);
            if (requestResult.IsError)
                return requestResult;

            //validate client
            var clientResult = ValidateClient(request, clientValidationResult);
            if (clientResult.IsError)
                return clientResult;

            //validate scope
            var scopeResult = await ValidateScopeAsync(request);
            if (scopeResult.IsError)
            {
                return scopeResult;
            }
            _logger.LogDebug("{clientId} anonnymous request validation success", request.Client.ClientId);
            return Valid(request);
        }

        private AuthorizationRequestValidationResult Valid(ValidatedAnonnymousAuthorizationRequest request) => new(request);

        private AuthorizationRequestValidationResult Invalid(
            ValidatedAnonnymousAuthorizationRequest request,
            string error = OidcConstants.AuthorizeErrors.InvalidRequest,
            string description = null) =>
            new(request, error, description);
        private AuthorizationRequestValidationResult ValidateRequestParameters(
            ValidatedAnonnymousAuthorizationRequest request)
        {
            var parameters = request.Raw;
            var transport = parameters[Constants.FormParameters.Transport];
            if (!transport.HasValue() || !_options.Transports.Contains(transport))
            {
                LogError($"Request Properties: missing or unsupportted value for {Constants.FormParameters.Transport}", request);
                return Invalid(request);
            }
            request.Transport = transport;

            request.Provider = parameters[Constants.FormParameters.Provider];
            if (!request.Provider.HasValue())
            {
                LogError($"Request Properties: missing or unsupportted value for {Constants.FormParameters.Provider}", request);
                return Invalid(request);
            }
            request.TransportData = parameters[Constants.FormParameters.TransportData]; ;

            request.State = parameters[Constants.FormParameters.State];

            return Valid(request);
        }
        private AuthorizationRequestValidationResult ValidateClient(
            ValidatedAnonnymousAuthorizationRequest request,
            ClientSecretValidationResult clientValidationResult)
        {
            //////////////////////////////////////////////////////////
            // set client & secret
            //////////////////////////////////////////////////////////
            if (clientValidationResult == default) throw new ArgumentNullException(nameof(clientValidationResult));
            request.SetClient(
                clientValidationResult.Client ?? throw new ArgumentNullException(nameof(clientValidationResult.Client)),
                clientValidationResult.Secret);

            //////////////////////////////////////////////////////////
            // check if client protocol type is oidc
            //////////////////////////////////////////////////////////
            if (request.Client.ProtocolType != IdentityServerConstants.ProtocolTypes.OpenIdConnect)
            {
                LogError("Invalid protocol type for OIDC authorize endpoint", request.Client.ProtocolType, request);
                return Invalid(request, OidcConstants.AuthorizeErrors.UnauthorizedClient, "Invalid protocol");
            }

            //////////////////////////////////////////////////////////
            // check if client allows anonnymous flow
            //////////////////////////////////////////////////////////
            if (!request.Client.AllowedGrantTypes.Contains(Constants.AnonnymousGrantType))
            {
                LogError("Client not configured for anonnymous flow", Constants.AnonnymousGrantType, request);
                return Invalid(request, OidcConstants.AuthorizeErrors.UnauthorizedClient);
            }

            //validate transport
            if (!request.Client.Properties.TryGetValue(Constants.ClientProperties.Transports, out var transports) ||
                !transports.HasValue() ||
                !transports.TryParseAsJsonDocument(out var doc))
            {
                LogError($"Client Properties: bad or missing data for {Constants.ClientProperties.Transports}", request);
                return Invalid(request);
            }

            var t = request.Raw[Constants.FormParameters.Transport];
            var matchingTransports = doc.RootElement.EnumerateArray()
                .Where(j => j.PropertyStringValueEqualsTo(Constants.ClientProperties.TransportName, t));

            if (matchingTransports.IsNullOrEmpty())
            {
                LogError($"Invalid or missing client transports", request);
                return Invalid(request);
            }

            var p = request.Raw[Constants.FormParameters.Provider];
            var matchingProvider = matchingTransports.FirstOrDefault(x => x.PropertyStringValueEqualsTo(Constants.ClientProperties.TransportProvider, p));

            if (matchingProvider.ValueKind == JsonValueKind.Undefined)
            {
                LogError($"Invalid or missing transport provider", request);
                return Invalid(request);
            }

            var redirectUri = request.Raw.Get(Constants.FormParameters.RedirectUri);
            if (!redirectUri.HasValue() ||
                !redirectUri.TryConvertToUri(out var redirectUriAsUri) ||
                !request.Client.RedirectUris.Any(r => uriEqual(new Uri(r), redirectUriAsUri)))
            {
                LogDebug($"Invalid redirect_uri: {redirectUri}", Constants.AnonnymousGrantType, request);
                return Invalid(request, OidcConstants.AuthorizeErrors.UnauthorizedClient);
            }
            request.RedirectUrl = redirectUri;

            request.VerificationUri = _options.VerificationUri;

            return Valid(request);

            static bool uriEqual(Uri u1, Uri u2)
            {
                var res = Uri.Compare(u1, u2, UriComponents.HttpRequestUrl, UriFormat.UriEscaped, StringComparison.InvariantCultureIgnoreCase);
                return res == 0;
            }
        }


        private async Task<AuthorizationRequestValidationResult> ValidateScopeAsync(ValidatedAnonnymousAuthorizationRequest request)
        {
            //////////////////////////////////////////////////////////
            // scope must be present
            //////////////////////////////////////////////////////////
            var scope = request.Raw.Get(OidcConstants.AuthorizeRequest.Scope);
            if (!scope.HasValue())
            {
                LogError("No allowed scopes configured for client", request);
                return Invalid(request, OidcConstants.AuthorizeErrors.InvalidScope);
            }

            if (scope.Length > _options.InputLengthRestrictions.Scope)
            {
                LogError("scopes too long.", request);
                return Invalid(request, description: "Invalid scope");
            }

            request.RequestedScopes = scope.FromDelimitedString(" ").Distinct().ToList();

            if (request.RequestedScopes.Contains(IdentityServerConstants.StandardScopes.OpenId))
            {
                request.IsOpenIdRequest = true;
            }

            //////////////////////////////////////////////////////////
            // check if scopes are valid/supported
            //////////////////////////////////////////////////////////
            var validatedResources = await _resourceValidator.ValidateRequestedResourcesAsync(new ResourceValidationRequest
            {
                Client = request.Client,
                Scopes = request.RequestedScopes
            });

            if (validatedResources == default || !validatedResources.Succeeded)
            {
                if (validatedResources?.InvalidScopes?.Count > 0)
                {
                    return Invalid(request, OidcConstants.AuthorizeErrors.InvalidScope);
                }

                return Invalid(request, OidcConstants.AuthorizeErrors.UnauthorizedClient, "Invalid scope");
            }

            if (validatedResources.Resources.IdentityResources.Any() && !request.IsOpenIdRequest)
            {
                LogError("Identity related scope requests, but no openid scope", request);
                return Invalid(request, OidcConstants.AuthorizeErrors.InvalidScope);
            }

            request.ValidatedResources = validatedResources;

            return Valid(request);
        }
        private void LogDebug(string message, string detail, ValidatedAnonnymousAuthorizationRequest request)
        {
            var requestDetails = new AnonnymousAuthorizationRequestValidationLog(request);
            _logger.LogDebug(message + ": {detail}\n{requestDetails}", detail, requestDetails);
        }
        private void LogError(string message, ValidatedAnonnymousAuthorizationRequest request)
        {
            var requestDetails = new AnonnymousAuthorizationRequestValidationLog(request);
            _logger.LogError(message + "\n{requestDetails}", requestDetails);
        }
        private void LogError(string message, string detail, ValidatedAnonnymousAuthorizationRequest request)
        {
            var requestDetails = new AnonnymousAuthorizationRequestValidationLog(request);
            _logger.LogError(message + ": {detail}\n{requestDetails}", detail, requestDetails);
        }
    }
}