using Dapper;
using IdentityServer4.Anonnymous.Services;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;
using static IdentityServer4.Anonnymous.Data.SqlScripts;

namespace IdentityServer4.Anonnymous.Data
{
    public class DefaultAnnonymousCodeStore : IAnnonymousCodeStore
    {
        private readonly Func<IDbConnection> _createDbConnection;
        private readonly ILogger<DefaultAnnonymousCodeStore> _logger;

        public DefaultAnnonymousCodeStore(
            Func<IDbConnection> createDbConnection,
            ILogger<DefaultAnnonymousCodeStore> logger)
        {
            _createDbConnection = createDbConnection;
            _logger = logger;
        }
        public async Task<AnonnymousCodeInfo> FindByUserCodeAsync(string code, bool includeExpiredAndVerified)
        {
            _logger.LogDebug($"start {nameof(FindByUserCodeAsync)}");
            if (!code.HasValue())
                return null;
            var query = includeExpiredAndVerified ?
                AnonnymousCodeScripts.SelectByUserCode :
                AnonnymousCodeScripts.SelectByUserCodeExcludeExpiredAndVerified;
            var ac = await GetSingleItem(query, new { UserCode = code.Sha256() });
            _logger.LogDebug($"{nameof(AnonnymousCodeInfo)} was returned from store: {ac.ToJsonString()}");
            return ac;
        }
        public async Task<AnonnymousCodeInfo> FindByVerificationCodeAsync(string code, bool includeExpiredAndVerified)
        {
            _logger.LogDebug($"start {nameof(FindByVerificationCodeAsync)}");
            var query = includeExpiredAndVerified ?
                AnonnymousCodeScripts.SelectByVeridicationCode :
                AnonnymousCodeScripts.SelectByVerificationCodeExcludeExpiredAndVerified;
            var ac = await GetSingleItem(query, new { VerificationCode = code.Sha256() });
            _logger.LogDebug($"{nameof(AnonnymousCodeInfo)} was returned from store: {ac.ToJsonString()}");
            return ac;
        }
        public async Task<AnonnymousCodeInfo> FindByVerificationCodeAndUserCodeAsync(string verificationCode, string userCode)
        {
            _logger.LogDebug($"start {nameof(FindByVerificationCodeAndUserCodeAsync)}");
            var query = AnonnymousCodeScripts.SelectByVerificationAndUserCodeExcludeExpiredAndVerified;
            var ac = await GetSingleItem(query, new { VerificationCode = verificationCode.Sha256(), UserCode = userCode.Sha256()});
            _logger.LogDebug($"{nameof(AnonnymousCodeInfo)} was returned from store: {ac.ToJsonString()}");
            return ac;
        }
        public async Task<AnonnymousCodeInfo> GetSingleItem(string query, object prms)
        {
            using var con = _createDbConnection();
            var model = await con.QuerySingleOrDefaultAsync<AnonnymousCodeDbModel>(query, prms);
            return model == null ? null : FromDbModel(model);
        }
        public async Task StoreAnonnymousCodeInfoAsync(string verificationCode, AnonnymousCodeInfo data)
        {
            _logger.LogInformation($"Start {nameof(StoreAnonnymousCodeInfoAsync)}");
            if (!verificationCode.HasValue()) throw new ArgumentNullException(nameof(verificationCode));
            //if (!anonnymouseCode.HasValue()) throw new ArgumentNullException(nameof(anonnymouseCode));
            if (data == default) throw new ArgumentNullException(nameof(data));

            _logger.LogDebug($"Storing anonnymous code: {data.ToJsonString()}");

            var dbModel = ToDbModel(data);
            dbModel.VerificationCode = verificationCode;
            using var con = _createDbConnection();
            await con.ExecuteAsync(AnonnymousCodeScripts.InsertCommand, dbModel);
        }
        public async Task UpdateVerificationRetryAsync(string verificationCode)
        {
            _logger.LogInformation($"Start {nameof(UpdateVerificationRetryAsync)}");
            if (!verificationCode.HasValue()) return;
            using var con = _createDbConnection();
            await con.ExecuteAsync(AnonnymousCodeScripts.UpdateVerificationRetry, new { VerificationCode = verificationCode.Sha256() });
        }

        #region Utilities and nested classes
        internal record AnonnymousCodeDbModel
        {
            public Guid Id { get; set; }
            public int AllowedRetries { get; set; }
            public string AuthorizedScopes { get; set; }
            public string ClientId { get; set; }
            public DateTime CreatedOnUtc { get; set; }
            public string Description { get; set; }
            public DateTime ExpiresOnUtc { get; set; }
            public int Lifetime { get; set; }
            public string RequestedScopes { get; set; }
            public int RetryCounter { get; set; }
            public string ReturnUrl { get; set; }
            public string Transport { get; set; }
            public string UserCode { get; set; }
            public string VerificationCode { get; set; }
            public DateTime VerifiedOnUtc { get; set; }
        }
        internal static AnonnymousCodeDbModel ToDbModel(AnonnymousCodeInfo info) => new()
        {
            Id = info.Id,
            AllowedRetries = info.AllowedRetries,
            AuthorizedScopes = info.AuthorizedScopes.ToDelimitedString(" "),
            ClientId = info.ClientId,
            CreatedOnUtc = info.CreatedOnUtc,
            Description = info.Description,
            ExpiresOnUtc = info.CreatedOnUtc.AddSeconds(info.Lifetime),
            Lifetime = info.Lifetime,
            RequestedScopes = info.RequestedScopes.ToDelimitedString(" "),
            ReturnUrl = info.ReturnUrl,
            Transport = info.Transport,
            UserCode = info.UserCode,
            VerificationCode = info.VerificationCode,
        };
        internal static AnonnymousCodeInfo FromDbModel(AnonnymousCodeDbModel model) => new()
        {
            Id = model.Id,
            AllowedRetries = model.AllowedRetries,
            VerificationCode = model.VerificationCode,
            AuthorizedScopes = model.AuthorizedScopes.FromDelimitedString(" "),
            ClientId = model.ClientId,
            CreatedOnUtc = model.CreatedOnUtc,
            Description = model.Description,
            RequestedScopes = model.RequestedScopes.FromDelimitedString(" "),
            RetryCounter = model.RetryCounter,
            ReturnUrl = model.ReturnUrl,
            Transport = model.Transport,
            VerifiedOnUtc = model.VerifiedOnUtc
        };
        #endregion
    }
}
