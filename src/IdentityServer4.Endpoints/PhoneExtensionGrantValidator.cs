﻿using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.PhoneAuthorizationEndpoint
{
    public class PhoneExtensionGrantValidator : IExtensionGrantValidator
    {
        private readonly ITokenValidator _tokenValidator;
        private readonly IPhoneCodeService _Phones;
        private readonly IPhoneCodeValidator _PhoneCodeValidator;
        private readonly ILogger<PhoneExtensionGrantValidator> _logger;

        public PhoneExtensionGrantValidator(
            ITokenValidator validator,
            IPhoneCodeService Phones,
            IPhoneCodeValidator PhoneCodeValidator,
            ILogger<PhoneExtensionGrantValidator> logger)
        {
            _tokenValidator = validator;
            _Phones = Phones;
            _PhoneCodeValidator = PhoneCodeValidator;
            _logger = logger;
        }

        public string GrantType => Constants.PhoneGrantType;
        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            _logger.LogInformation($"Start Extension grant validation for {GrantType}");
            var token = context?.Request?.Raw?.Get("token");
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

            var code = context.Request.Raw.Get("Phone_code");
            if (!code.HasValue())
            {
                Error(context, "Invalid Phone code");
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
            var PhoneCode = await _Phones.FindByPhoneCodeAsync(code);
            //validate Phone code
            var PhoneValidRequest = new PhoneCodeValidationRequest(PhoneCode, tokenValidationResult.Client, subject);
            var PhoneValidResult = await _PhoneCodeValidator.ValidateVerifiedPhoneCodeAsync(PhoneValidRequest);
            if (PhoneValidResult.IsError)
            {
                Error(context, "Invalid Phone code");
                return;
            }

            var am = $"{amr} {PhoneCode.Transport}";
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
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
        }
    }
    {
    }
}