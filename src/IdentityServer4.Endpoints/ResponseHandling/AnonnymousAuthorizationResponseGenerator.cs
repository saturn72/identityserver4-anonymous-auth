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

namespace IdentityServer4.Anonnymous.ResponseHandling
{
    public class AnonnymousAuthorizationResponseGenerator : IAuthorizationResponseGenerator
    {
        #region fields
        private readonly IUserCodeService _userCodeService;
        private readonly AnonnymousAuthorizationOptions _options;
        private readonly IAnonnymousCodeService _anonnymousCodeService;
        private readonly ISystemClock _clock;
        private readonly IHandleGenerationService _handleGenerationService;
        private readonly ILogger<AnonnymousAuthorizationResponseGenerator> _logger;
        #endregion

        #region ctor
        public AnonnymousAuthorizationResponseGenerator(
            IUserCodeService userCodeService,
            IOptions<AnonnymousAuthorizationOptions> options,
            IAnonnymousCodeService anonnymousCodeService,
            ISystemClock clock,
            IHandleGenerationService handleGenerationService,
            ILogger<AnonnymousAuthorizationResponseGenerator> logger)
        {
            _userCodeService = userCodeService;
            _options = options.Value;
            _anonnymousCodeService = anonnymousCodeService;
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

            //// generate user_code
            //var generator = await _userCodeService.GetGenerator(client.UserCodeType ?? _options.DefaultUserCodeType);
            //var retryCount = 0;
            //while (retryCount < generator.RetryLimit)
            //{
            //    var sessionCode = await generator.GenerateAsync();
            //    var storedCode = await _anonnymousCodeService.FindByUserCodeAsync(sessionCode, true);
            //    if (storedCode == null)
            //    {
            //        response.AnonnymousCode = sessionCode;
            //        break;
            //    }
            //    retryCount++;
            //}
            //if (!response.UserCode.HasValue())
            //{
            //    throw new InvalidOperationException("Unable to create unique phone flow user code");
            //}
            _logger.LogDebug($"user code was generated with value: {response.UserCode}");

            var anonnymousCode = await _handleGenerationService.GenerateAsync();
            _logger.LogDebug($"anonnymous-code was generated valued: {anonnymousCode}");
            response.AnonnymousCode = anonnymousCode;
            // generate activation URIs
            response.ActivationUri = _options.ActivationUri;
            response.ActivationUriComplete = $"{_options.ActivationUri.RemoveTrailingSlash()}?{Constants.IdentityModel.AnonnymousCode}={response.AnonnymousCode}";

            // lifetime
            response.AnonnymousCodeLifetime = client.TryGetIntPropertyOrDefault(_options.LifetimePropertyName, _options.DefaultInteractionCodeLifetime);
            _logger.LogDebug($"phone lifetime was set to {response.AnonnymousCodeLifetime}");

            // interval
            response.Interval = _options.Interval;
            _logger.LogDebug($"phone interval was set to {response.Interval}");

            //allowed retries
            var allowedRetries = client.TryGetIntPropertyOrDefault(_options.AllowedRetriesPropertyName, _options.AllowedRetries);

            response.Retries = allowedRetries;
            _logger.LogDebug($"Max allowed retries was set to {allowedRetries}");


            var ac = new AnonnymousCodeInfo
            {
                AllowedRetries = allowedRetries,
                AnonnymousCode = response.AnonnymousCode,
                ClientId = client.ClientId,
                CreatedOnUtc = _clock.UtcNow.UtcDateTime,
                Description = validatedRequest.Description,
                IsOpenId = validatedRequest.IsOpenIdRequest,
                Lifetime = response.AnonnymousCodeLifetime,
                Transport = validatedRequest.Transport,
                TransportProvider = validatedRequest.Provider,
                TransportData = validatedRequest.TransportData,
                ReturnUrl = validatedRequest.RedirectUrl,
                RequestedScopes = validatedRequest.RequestedScopes,
                VerificationUri = validatedRequest.VerificationUri,
            };
            _logger.LogDebug($"storing anonnymous-code in database: {ac.ToJsonString()}");
            await _anonnymousCodeService.StoreAnonnymousCodeInfoAsync(response.AnonnymousCode, ac);
            return response;
        }
    }

}