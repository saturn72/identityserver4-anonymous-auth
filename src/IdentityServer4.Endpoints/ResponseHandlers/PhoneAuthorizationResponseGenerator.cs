using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.PhoneAuthorizationEndpoint.Services;
using IdentityServer4.PhoneAuthorizationEndpoint.Validation;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace IdentityServer4.PhoneAuthorizationEndpoint.ResponseHandlers
{
    public class PhoneAuthorizationResponseGenerator : IPhoneAuthorizationResponseGenerator
    {
        #region fields
        private readonly IUserCodeService _userCodeService;
        private readonly PhoneAuthorizationOptions _options;
        private readonly IPhoneCodeService _phoneCodeService;
        private readonly ISystemClock _clock;
        private readonly IHandleGenerationService _handleGenerationService;
        private readonly ILogger<PhoneAuthorizationResponseGenerator> _logger;
        #endregion

        #region ctor
        public PhoneAuthorizationResponseGenerator(
            IUserCodeService userCodeService,
            IOptions<PhoneAuthorizationOptions> options,
            IPhoneCodeService phoneCodeService,
            ISystemClock clock,
            IHandleGenerationService handleGenerationService,
            ILogger<PhoneAuthorizationResponseGenerator> logger)
        {
            _userCodeService = userCodeService;
            _options = options.Value;
            _phoneCodeService = phoneCodeService;
            _clock = clock;
            _handleGenerationService = handleGenerationService;
            _logger = logger;

        }
        #endregion
        public async Task<PhoneAuthorizationResponse> ProcessAsync(PhoneAuthorizationRequestValidationResult validationResult, string baseUrl)
        {
            _logger.LogInformation("start Processing Phone request");
            if (validationResult == null) throw new ArgumentNullException(nameof(validationResult));
            if (!baseUrl.HasValue()) throw new ArgumentException("Value cannot be null or whitespace.", nameof(baseUrl));

            var validatedRequest = validationResult.ValidatedRequest;
            var client = validatedRequest?.Client;
            if (client == null) throw new ArgumentNullException(nameof(validationResult.ValidatedRequest.Client));

            _logger.LogDebug("Creating response for phone authorization request");
            var response = new PhoneAuthorizationResponse();

            // generate user_code
            var userCodeGenerator = await _userCodeService.GetGenerator(client.UserCodeType ?? _options.DefaultUserCodeType);
            var retryCount = 0;
            while (retryCount < userCodeGenerator.RetryLimit)
            {
                var userCode = await userCodeGenerator.GenerateAsync();
                var storePhoneCode = await _phoneCodeService.FindByUserCodeAsync(userCode);
                if (storePhoneCode == null)
                {
                    response.UserCode = userCode;
                    break;
                }
                retryCount++;
            }
            if (!response.UserCode.HasValue())
            {
                throw new InvalidOperationException("Unable to create unique phone flow user code");
            }
            _logger.LogDebug($"user code was generated with value: {response.UserCode}");

            // generate verification URIs
            response.VerificationUri = _options.PhoneVerificationUrl;
            if (response.VerificationUri.IsLocalUrl())
            {
                // if url is relative, parse absolute URL
                response.VerificationUri = baseUrl.RemoveTrailingSlash() + _options.PhoneVerificationUrl;
            }
            // lifetime
            var defaultLifetime = _options.DefaultInteractionCodeLifetime;

            response.PhoneCodeLifetime = client.TryGetIntPropertyOrDefault(_options.LifetimePropertyName, defaultLifetime);
            _logger.LogDebug($"phone lifetime was set to {response.PhoneCodeLifetime}");

            // interval
            response.Interval = _options.Interval;
            _logger.LogDebug($"phone interval was set to {response.Interval}");

            //allowed retries
            var allowedRetries = client.TryGetIntPropertyOrDefault(_options.AllowedRetriesPropertyName, _options.AllowedRetries);

            response.Retries = allowedRetries;
            _logger.LogDebug($"Max allowed retries was set to {allowedRetries}");

            var phoneCode = await _handleGenerationService.GenerateAsync();
            response.PhoneCode = phoneCode;
            _logger.LogDebug($"phone-code was generated valued: {phoneCode}");
            var mc = new PhoneCode
            {
                AllowUserCodeTransfer = client.TryGetBooleanPropertyOrDefault(Constants.ClientProperties.AllowUserCodeTransfer),
                ClientId = client.ClientId,
                CreatedOnUtc = _clock.UtcNow.UtcDateTime,
                Description = validatedRequest.Description,
                IsOpenId = validatedRequest.IsOpenIdRequest,
                Lifetime = response.PhoneCodeLifetime,
                RequestSubjectId = validatedRequest.Subject.GetSubjectId(),
                RequestIdentityProvider = validatedRequest.Subject.GetIdentityProvider(),
                Transport = validatedRequest.Transport,
                ReturnUrl = validatedRequest.ReturnUrl,
                AllowedRetries = allowedRetries,
            };
            _logger.LogDebug($"storing phone-code in database: {mc.ToJsonString()}");
            await _phoneCodeService.StorePhoneAuthorizationAsync(response.UserCode, phoneCode, mc);
            return response;
        }
    }

}