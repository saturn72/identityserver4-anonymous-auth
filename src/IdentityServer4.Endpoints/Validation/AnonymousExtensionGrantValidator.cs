using IdentityModel;
using IdentityServer4.Anonymous.Stores;
using IdentityServer4.Anonymous.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Stores;

namespace IdentityServer4.Anonymous.Validation
{
    public class AnonymousExtensionGrantValidator : IExtensionGrantValidator
    {
        private readonly ITokenValidator _tokenValidator;
        private readonly IAnonymousCodeStore _codeStore;
        private readonly IAnonymousCodeValidator _anonymousCodeValidator;
        private readonly IClientStore _clients;
        private readonly ILogger<AnonymousExtensionGrantValidator> _logger;

        public AnonymousExtensionGrantValidator(
            ITokenValidator validator,
            IAnonymousCodeStore codeStore,
            IAnonymousCodeValidator anonymousCodeValidator,
            IClientStore clients,
            ILogger<AnonymousExtensionGrantValidator> logger)
        {
            _tokenValidator = validator;
            _codeStore = codeStore;
            _anonymousCodeValidator = anonymousCodeValidator;
            _logger = logger;
            _clients = clients;
        }

        public string GrantType => Constants.AnonymousGrantType;
        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            _logger.LogInformation($"Start Extension grant validation for {GrantType}");

            var code = context.Request.Raw.Get(Constants.UserInteraction.VerificationCode);
            if (!code.HasValue())
            {
                Error(context, "Invalid User code");
                return;
            }

            string amr;
            var ac = await _codeStore.FindByVerificationCodeAsync(code, false);
            if (ac == default ||
                !ac.IsAuthorized ||
                ac.Claims.IsNullOrEmpty() ||
            !(amr = ac.Claims.GetFirstValueOrDefault(JwtClaimTypes.AuthenticationMethod)).HasValue())
            {
                Error(context, $"Failed to fetch verification code using \'code\' = {code}");
                return;
            }

            var subject = Principal.Create(amr, ac.Claims.ToArray());

            var client = await _clients.FindClientByIdAsync(ac.ClientId);
            if (client == default)
            {
                Error(context, $"Failed to fetch client for \'code\' = {code}");
                return;
            }
            //validate code
            var validationRequest = new AnonymousCodeValidationRequest(ac, client, subject);
            var validationResult = await _anonymousCodeValidator.ValidateVerifiedPhoneCodeAsync(validationRequest);
            if (validationResult.IsError)
            {
                Error(context, "Invalid User code");
                return;
            }

            var am = $"{amr} {ac.Transport}";
            var provider = ac.Claims.GetFirstValueOrDefault(JwtClaimTypes.IdentityProvider) ??
                throw new ArgumentNullException(JwtClaimTypes.IdentityProvider);
            var subjectId = ac.Subject ??
                throw new ArgumentNullException(nameof(AnonymousCodeInfo.Subject));

            context.Result = new GrantValidationResult(
                identityProvider: provider,
                subject: subjectId,
                authenticationMethod: am,
                claims: ac.Claims
                );
            return;
        }

        private void Error(ExtensionGrantValidationContext context, string message)
        {
            _logger.LogError(message);
            context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidGrant);
        }
    }
}
