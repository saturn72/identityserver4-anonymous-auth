using IdentityModel;
using IdentityServer4.Anonymous.Stores;
using IdentityServer4.Anonymous.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Anonymous.Validation
{
    public class AnonymousExtensionGrantValidator : IExtensionGrantValidator
    {
        private readonly ITokenValidator _tokenValidator;
        private readonly IAnonymousCodeStore _codeStore;
        private readonly IAnonymousCodeValidator _anonymousCodeValidator;
        private readonly ILogger<AnonymousExtensionGrantValidator> _logger;

        public AnonymousExtensionGrantValidator(
            ITokenValidator validator,
            IAnonymousCodeStore codeStore,
            IAnonymousCodeValidator anonymousCodeValidator,
            ILogger<AnonymousExtensionGrantValidator> logger)
        {
            _tokenValidator = validator;
            _codeStore = codeStore;
            _anonymousCodeValidator = anonymousCodeValidator;
            _logger = logger;
        }

        public string GrantType => Constants.AnonymousGrantType;
        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            _logger.LogInformation($"Start Extension grant validation for {GrantType}");
            var token = context.Request.Raw.Get("token");
            if (!token.HasValue())
            {
                Error(context, "Invalid token");
                return;
            }
            var tokenValidationResult = await _tokenValidator.ValidateAccessTokenAsync(token);
            if (tokenValidationResult.IsError)
            {
                Error(context, "Invalid token");
                return;
            }

            var code = context.Request.Raw.Get("user_code");
            if (!code.HasValue())
            {
                Error(context, "Invalid User code");
                return;
            }
            string amr;
            if (tokenValidationResult.Claims.IsNullOrEmpty() ||
                !(amr = tokenValidationResult.Claims.GetFirstValueOrDefault(JwtClaimTypes.AuthenticationMethod)).HasValue())
            {
                Error(context, "Invalid user amr");
                return;
            }

            var subject = Principal.Create(amr, tokenValidationResult.Claims.ToArray());
            var ac = await _codeStore.FindByVerificationCodeAsync(code, false);
            if (ac == default)
            {
                Error(context, $"Failed to fetch verification code using \'code\' = {code}");
                return;
            }
            //validate Phone code
            var validationRequest = new AnonymousCodeValidationRequest(ac, tokenValidationResult.Client, subject);
            var validationResult = await _anonymousCodeValidator.ValidateVerifiedPhoneCodeAsync(validationRequest);
            if (validationResult.IsError)
            {
                Error(context, "Invalid Phone code");
                return;
            }

            var am = $"{amr} {ac.Transport}";
            var provider = tokenValidationResult.Claims.GetFirstValueOrDefault(JwtClaimTypes.IdentityProvider);
            var subjectId = tokenValidationResult.Claims.GetFirstValueOrDefault(JwtClaimTypes.Subject);

            context.Result = new GrantValidationResult(
                identityProvider: provider,
                subject: subjectId,
                authenticationMethod: am
                //claims: result.Claims, ==> get user claims here
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
