using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Linq;
using IdentityServer4.PhoneAuthorizationEndpoint.Models;
using IdentityServer4.PhoneAuthorizationEndpoint.Validation;
using IdentityModel;
using System.Collections.Generic;
using IdentityServer4.PhoneAuthorizationEndpoint.Logging;

namespace IdentityServer4.PhoneAuthorizationEndpoint
{
    public sealed class PhoneAuthorizationRequestValidator : IPhoneAuthorizationRequestValidator
    {
        private readonly PhoneAuthorizationOptions _options;
        private readonly IResourceValidator _resourceValidator;
        private readonly ILogger<PhoneAuthorizationRequestValidator> _logger;

        public PhoneAuthorizationRequestValidator(
            IOptions<PhoneAuthorizationOptions> options,
            IResourceValidator resourceValidator, 
            ILogger<PhoneAuthorizationRequestValidator> logger)
        {
            _options = options.Value;
            _resourceValidator = resourceValidator;
            _logger = logger;
        }
        public async Task<PhoneAuthorizationRequestValidationResult> ValidateAsync(NameValueCollection parameters, ClientSecretValidationResult clientValidationResult)
        {
            _logger.LogDebug("Start phone authorization request validation");

            var request = new ValidatedPhoneAuthorizationRequest
            {
                Raw = parameters ?? throw new ArgumentNullException(nameof(parameters)),
            };

            var transport = parameters[Constants.FormParameters.Transport];
            if (!transport.HasValue() || !_options.TransportOptions.Contains(transport))
                return Invalid(request);
            request.Transport = transport;

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
            _logger.LogDebug("{clientId} Phone request validation success", request.Client.ClientId);
            return Valid(request);
            throw new System.NotImplementedException();
        }
        private PhoneAuthorizationRequestValidationResult Valid(ValidatedPhoneAuthorizationRequest request)
        {
            return new PhoneAuthorizationRequestValidationResult(request);
        }

        private PhoneAuthorizationRequestValidationResult Invalid(ValidatedPhoneAuthorizationRequest request, string error = OidcConstants.AuthorizeErrors.InvalidRequest, string description = null)
        {
            return new PhoneAuthorizationRequestValidationResult(request, error, description);
        }
        private PhoneAuthorizationRequestValidationResult ValidateClient(ValidatedPhoneAuthorizationRequest request, ClientSecretValidationResult clientValidationResult)
        {
            //////////////////////////////////////////////////////////
            // set client & secret
            //////////////////////////////////////////////////////////
            if (clientValidationResult == null) throw new ArgumentNullException(nameof(clientValidationResult));
            request.SetClient(clientValidationResult.Client, clientValidationResult.Secret);

            //////////////////////////////////////////////////////////
            // check if client protocol type is oidc
            //////////////////////////////////////////////////////////
            if (request.Client.ProtocolType != IdentityServerConstants.ProtocolTypes.OpenIdConnect)
            {
                LogError("Invalid protocol type for OIDC authorize endpoint", request.Client.ProtocolType, request);
                return Invalid(request, OidcConstants.AuthorizeErrors.UnauthorizedClient, "Invalid protocol");
            }

            //////////////////////////////////////////////////////////
            // check if client allows phone flow
            //////////////////////////////////////////////////////////
            if (!request.Client.AllowedGrantTypes.Contains(Constants.PhoneGrantType))
            {
                LogError("Client not configured for phone flow", Constants.PhoneGrantType, request);
                return Invalid(request, OidcConstants.AuthorizeErrors.UnauthorizedClient);
            }

            return Valid(request);
        }

        private async Task<PhoneAuthorizationRequestValidationResult> ValidateScopeAsync(ValidatedPhoneAuthorizationRequest request)
        {
            //////////////////////////////////////////////////////////
            // scope must be present
            //////////////////////////////////////////////////////////
            var scope = request.Raw.Get(OidcConstants.AuthorizeRequest.Scope);
            if (!scope.HasValue())
            {
                _logger.LogTrace("Client provided no scopes - checking allowed scopes list");

                if (!request.Client.AllowedScopes.IsNullOrEmpty())
                {
                    var clientAllowedScopes = new List<string>(request.Client.AllowedScopes);
                    if (request.Client.AllowOfflineAccess)
                    {
                        clientAllowedScopes.Add(IdentityServerConstants.StandardScopes.OfflineAccess);
                    }
                    scope = clientAllowedScopes.ToDelimitedString(" ");
                    _logger.LogTrace("Defaulting to: {scopes}", scope);
                }
                else
                {
                    LogError("No allowed scopes configured for client", request);
                    return Invalid(request, OidcConstants.AuthorizeErrors.InvalidScope);
                }
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

            if (!validatedResources.Succeeded)
            {
                if (validatedResources.InvalidScopes.Count > 0)
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
        private void LogError(string message, ValidatedPhoneAuthorizationRequest request)
        {
            var requestDetails = new PhoneAuthorizationRequestValidationLog(request);
            _logger.LogError(message + "\n{requestDetails}", requestDetails);
        }
        private void LogError(string message, string detail, ValidatedPhoneAuthorizationRequest request)
        {
            var requestDetails = new PhoneAuthorizationRequestValidationLog(request);
            _logger.LogError(message + ": {detail}\n{requestDetails}", detail, requestDetails);
        }
    }
}