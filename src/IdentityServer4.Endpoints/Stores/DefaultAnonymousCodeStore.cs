using Dapper;
using IdentityServer4.Anonymous.Services;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using static IdentityServer4.Anonymous.Stores.SqlScripts;

namespace IdentityServer4.Anonymous.Stores
{
    public class DefaultAnonymousCodeStore : IAnonymousCodeStore
    {
        private readonly Func<IDbConnection> _createDbConnection;
        private readonly ILogger<DefaultAnonymousCodeStore> _logger;

        public DefaultAnonymousCodeStore(
            Func<IDbConnection> createDbConnection,
            ILogger<DefaultAnonymousCodeStore> logger)
        {
            _createDbConnection = createDbConnection;
            _logger = logger;
        }
        public async Task<AnonymousCodeInfo> FindByUserCodeAsync(string code, bool includeExpiredAndVerified)
        {
            _logger.LogDebug($"start {nameof(FindByUserCodeAsync)}");
            if (!code.HasValue())
                return null;
            var query = includeExpiredAndVerified ?
                AnonymousCodeScripts.SelectByUserCode :
                AnonymousCodeScripts.SelectByUserCodeExcludeExpiredAndVerified;
            var ac = await GetSingleItem(query, new { UserCode = code.Sha256() });
            _logger.LogDebug($"{nameof(AnonymousCodeInfo)} was returned from store: {ac.ToJsonString()}");
            return ac;
        }
        public async Task<AnonymousCodeInfo> FindByVerificationCodeAsync(string code, bool includeExpiredAndVerified)
        {
            _logger.LogDebug($"start {nameof(FindByVerificationCodeAsync)}");
            if (!code.HasValue())
                return null;
            var query = includeExpiredAndVerified ?
                AnonymousCodeScripts.SelectByVeridicationCode :
                AnonymousCodeScripts.SelectByVerificationCodeExcludeExpiredAndVerified;
            var ac = await GetSingleItem(query, new { VerificationCode = code.Sha256() });
            _logger.LogDebug($"{nameof(AnonymousCodeInfo)} was returned from store: {ac.ToJsonString()}");
            return ac;
        }
        public async Task<AnonymousCodeInfo> FindByVerificationCodeAndUserCodeAsync(string verificationCode, string userCode)
        {
            _logger.LogDebug($"start {nameof(FindByVerificationCodeAndUserCodeAsync)}");
            if (!verificationCode.HasValue() || !userCode.HasValue())
                return null;

            var query = AnonymousCodeScripts.SelectByVerificationAndUserCodeExcludeExpiredAndVerified;
            var ac = await GetSingleItem(query, new { VerificationCode = verificationCode.Sha256(), UserCode = userCode.Sha256() });
            _logger.LogDebug($"{nameof(AnonymousCodeInfo)} was returned from store: {ac.ToJsonString()}");
            return ac;
        }
        private async Task<AnonymousCodeInfo> GetSingleItem(string query, object prms)
        {
            using var con = _createDbConnection();
            var model = await con.QuerySingleOrDefaultAsync<AnonymousCodeDbModel>(query, prms);
            return model == null ? null : FromDbModel(model);
        }
        public async Task StoreAnonymousCodeInfoAsync(string verificationCode, AnonymousCodeInfo info)
        {
            _logger.LogInformation($"Start {nameof(StoreAnonymousCodeInfoAsync)}");
            if (!verificationCode.HasValue()) throw new ArgumentException(nameof(verificationCode));
            if (info == default) throw new ArgumentNullException(nameof(info));

            _logger.LogDebug($"Storing anonymous code: {info.ToJsonString()}");

            var dbModel = ToDbModel(info);
            dbModel.VerificationCode = verificationCode.Sha256();
            using var con = _createDbConnection();
            await con.ExecuteAsync(AnonymousCodeScripts.InsertCommand, dbModel);
        }
        public async Task UpdateVerificationRetryAsync(string verificationCode)
        {
            _logger.LogInformation($"Start {nameof(UpdateVerificationRetryAsync)}");
            if (!verificationCode.HasValue()) return;
            using var con = _createDbConnection();
            await con.ExecuteAsync(AnonymousCodeScripts.UpdateVerificationRetry, new { VerificationCode = verificationCode.Sha256() });
        }

        public async Task<IEnumerable<string>> GetAllSubjectIdsByClientIdAsync(string clientId)
        {
            var query = AnonymousCodeScripts.SelectSubjectByClientId;
            using var con = _createDbConnection();
            return await con.QueryAsync<string>(query, new { clientId });
        }
        public async Task PrepareForAuthorizationUpdate(AnonymousCodeInfo code)
        {
            _logger.LogInformation($"Start {nameof(PrepareForAuthorizationUpdate)}");
            if (code == default || code.Claims.IsNullOrEmpty())
            {
                _logger.LogDebug("Invlaid data: {code}: ", code);
                return;
            }
            var claims = JsonSerializer.Serialize(code.Claims.Select(x => new { type = x.Type, @value = x.Value }));
            var prms = new
            {
                code.VerificationCode,
                Claims = claims,
                code.Subject,
            };

            using var con = _createDbConnection();
            await con.ExecuteAsync(AnonymousCodeScripts.UpdateAuthorization, prms);
        }
        #region Utilities and nested classes
        public record AnonymousCodeDbModel
        {
            public Guid Id { get; set; }
            public int AllowedRetries { get; set; }
            public string AuthorizedScopes { get; set; }
            public string Claims { get; set; }
            public string ClientId { get; set; }
            public DateTime CreatedOnUtc { get; set; }
            public string Description { get; set; }
            public DateTime ExpiresOnUtc { get; set; }
            public bool IsAuthorized { get; set; }
            public int Lifetime { get; set; }
            public string RequestedScopes { get; set; }
            public int RetryCounter { get; set; }
            public string ReturnUrl { get; set; }
            public string Subject { get; set; }
            public string Transport { get; set; }
            public string UserCode { get; set; }
            public string VerificationCode { get; set; }
            public DateTime VerifiedOnUtc { get; set; }
        }
        internal static AnonymousCodeDbModel ToDbModel(AnonymousCodeInfo info) => new()
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
        internal static AnonymousCodeInfo FromDbModel(AnonymousCodeDbModel model) =>
            new()
            {
                Id = model.Id,
                AllowedRetries = model.AllowedRetries,
                Claims = model.Claims.HasValue() ? JsonSerializer.Deserialize<Claim[]>(model.Claims) : default,
                ClientId = model.ClientId,
                CreatedOnUtc = model.CreatedOnUtc,
                Description = model.Description,
                IsAuthorized = model.IsAuthorized,
                RequestedScopes = model.RequestedScopes.FromDelimitedString(" "),
                RetryCounter = model.RetryCounter,
                ReturnUrl = model.ReturnUrl,
                Transport = model.Transport,
                VerificationCode = model.VerificationCode,
                VerifiedOnUtc = model.VerifiedOnUtc
            };

        #endregion
    }
}
