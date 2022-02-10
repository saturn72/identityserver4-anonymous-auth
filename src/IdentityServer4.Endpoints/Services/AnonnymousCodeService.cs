using IdentityServer4.Anonnymous.Data;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Services
{
    public class AnonnymousCodeService : IAnonnymousCodeService
    {
        private readonly IAnnonymousCodeStore _codes;
        private readonly ISystemClock _clock;
        private readonly ILogger<AnonnymousCodeService> _logger;
        public AnonnymousCodeService(IAnnonymousCodeStore codes,
            ISystemClock clock,
            ILogger<AnonnymousCodeService> logger)
        {
            _codes = codes;
            _clock = clock;
            _logger = logger;
        }
        public async Task<AnonnymousCodeInfo> FindByUserCodeAsync(string code, bool showExpired = false)
        {
            _logger.LogDebug($"start {nameof(FindByUserCodeAsync)}");
            var ac = await _codes.FindByUserCodeAsync(code.Sha256(), showExpired);
            _logger.LogDebug($"{nameof(AnonnymousCodeInfo)} was returned from store: {ac.ToJsonString()}");
            return ac;
        }
        public async Task<AnonnymousCodeInfo> FindByVerificationCodeAsync(string code, bool showExpired = false)
        {
            _logger.LogDebug($"start {nameof(FindByVerificationCodeAsync)}");
            var ac = await _codes.FindByVerificationCodeAsync(code.Sha256(), showExpired);
            _logger.LogDebug($"{nameof(AnonnymousCodeInfo)} was returned from store: {ac.ToJsonString()}");
            return ac;
        }

        public async Task<AnonnymousCodeInfo> FindByVerificationCodeAndUserCodeAsync(string verificationCode, string userCode)
        {
            _logger.LogDebug($"start {nameof(FindByVerificationCodeAndUserCodeAsync)}");
            var ac = await _codes.FindByVerificationCodeAndUserCodeAsync(verificationCode.Sha256(), userCode.Sha256());
            _logger.LogDebug($"{nameof(AnonnymousCodeInfo)} was returned from store: {ac.ToJsonString()}");
            return ac;
        }

        public async Task StoreAnonnymousCodeInfoAsync(string anonnymousCode, AnonnymousCodeInfo data)
        {
            _logger.LogInformation($"Start {nameof(StoreAnonnymousCodeInfoAsync)}");
            if (!anonnymousCode.HasValue()) throw new ArgumentNullException(nameof(anonnymousCode));
            //if (!anonnymouseCode.HasValue()) throw new ArgumentNullException(nameof(anonnymouseCode));
            if (data == default) throw new ArgumentNullException(nameof(data));

            _logger.LogDebug($"Storing anonnymous code: {data.ToJsonString()}");
            await _codes.StoreAnonnymousCodeInfoAsync(anonnymousCode.Sha256(), data);
        }
        public async Task UpdateVerificationRetryAsync(string verificationCode)
        {
            _logger.LogInformation($"Start {nameof(UpdateVerificationRetryAsync)}");
            if (!verificationCode.HasValue()) return;
            await _codes.UpdateVerificationRetryAsync(verificationCode.Sha256());
        }
    }
}
