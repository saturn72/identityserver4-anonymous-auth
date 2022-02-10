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
        public async Task<AnonnymousCodeInfo> FindByAnonnymousCodeAsync(string code, bool showExpired = false)
        {
            _logger.LogDebug($"start {nameof(FindByAnonnymousCodeAsync)}");
            var ac = await _codes.FindByAnonnymousCodeAsync(code.Sha256(), showExpired);
            _logger.LogDebug($"{nameof(AnonnymousCodeInfo)} was returned from store: {ac.ToJsonString()}");
            return ac;
        }

        public async Task<AnonnymousCodeInfo> FindByUserCodeAsync(string userCode, bool showExpired = false)
        {
            _logger.LogDebug($"start {nameof(FindByUserCodeAsync)}");
            var ac = await _codes.FindByUserCodeAsync(userCode.Sha256(), showExpired);
            _logger.LogDebug($"{nameof(AnonnymousCodeInfo)} was returned from store: {ac.ToJsonString()}");
            return ac;
        }

        public Task StoreAnonnymousCodeInfoAsync(string anonnymousCode, AnonnymousCodeInfo data)
        {
            _logger.LogInformation($"Start {nameof(StoreAnonnymousCodeInfoAsync)}");
            if (!anonnymousCode.HasValue()) throw new ArgumentNullException(nameof(anonnymousCode));
            //if (!anonnymouseCode.HasValue()) throw new ArgumentNullException(nameof(anonnymouseCode));
            if (data == default) throw new ArgumentNullException(nameof(data));

            _logger.LogDebug($"Storing anonnymous code: {data.ToJsonString()}");
            return _codes.StoreAnonnymousCodeInfoAsync(anonnymousCode.Sha256(), data);
        }

        public Task UpdateVerificationApprovedAsync(Guid id, AnonnymousCodeInfo data)
        {
            throw new NotImplementedException();
        }

        public Task UpdateVerificationRetryAsync(Guid id, string provider, string subjectId)
        {
            throw new NotImplementedException();
        }
        public async Task Activate(Guid id, string code)
        {
            _logger.LogInformation($"Start {nameof(Activate)}");
            if (id == default || !code.HasValue()) return;

            await _codes.Activate(id, code.Sha256());
        }
    }
}
