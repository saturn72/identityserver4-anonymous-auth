using Dapper;
using IdentityServer4.Anonnymous.Services;
using System;
using System.Data;
using System.Threading.Tasks;
using static IdentityServer4.Anonnymous.Data.SqlScripts;

namespace IdentityServer4.Anonnymous.Data
{
    public class DapperAnnonymousCodeStore : IAnnonymousCodeStore
    {
        private readonly Func<IDbConnection> _createDbConnection;

        public DapperAnnonymousCodeStore(Func<IDbConnection> createDbConnection)
        {
            _createDbConnection = createDbConnection;
        }
        public async Task<AnonnymousCodeInfo> FindByAnonnymousCodeAsync(string code, bool includeExpiredAndVerified)
        {
            var query = includeExpiredAndVerified ?
                AnonnymousCodeScripts.SelectByAnonnymousCode :
                AnonnymousCodeScripts.SelectByAnonnymousCodeExcludeExpiredActivatedAndVerified;
            return await GetSingleItem(query, new { AnonnymousCode = code });
        }

        public async Task<AnonnymousCodeInfo> FindByUserCodeAsync(string userCode, bool includeExpiredAndVerified)
        {
            var query = includeExpiredAndVerified ?
                AnonnymousCodeScripts.SelectByUserCode :
                AnonnymousCodeScripts.SelectByUserCodeExcludeExpiredActivatedAndVerified;
            return await GetSingleItem(query, new { UserCode = userCode });
        }
        public async Task<AnonnymousCodeInfo> GetSingleItem(string query, object prms)
        {
            using var con = _createDbConnection();
            var model = await con.QuerySingleOrDefaultAsync<AnonnymousCodeDbModel>(query, prms);
            return model == null ? null : FromDbModel(model);
        }
        public async Task StoreAnonnymousCodeInfoAsync(string anonnymousCode, AnonnymousCodeInfo data)
        {
            var dbModel = ToDbModel(data);
            dbModel.AnonnymousCode = anonnymousCode;
            using var con = _createDbConnection();
            await con.ExecuteAsync(AnonnymousCodeScripts.InsertCommand, dbModel);
        }
        public async Task Activate(Guid id, string userCode)
        {
            using var con = _createDbConnection();
            await con.ExecuteAsync(AnonnymousCodeScripts.Activate, new { Id = id, UserCode = userCode });
        }

        #region Utilities and nested classes
        internal record AnonnymousCodeDbModel
        {
            public Guid Id { get; set; }
            public DateTime ActivatedOnUtc { get; set; }
            public int AllowedVerificationRetries { get; set; }
            public string AuthorizedScopes { get; set; }
            public string ClientId { get; set; }
            public DateTime CreatedOnUtc { get; set; }
            public string Description { get; set; }
            public DateTime ExpiresOnUtc { get; set; }
            public bool IsOpenId { get; set; }
            public int Lifetime { get; set; }
            public string AnonnymousCode { get; set; }
            public string RequestedScopes { get; set; }
            public int VerificationRetryCounter { get; set; }
            public string ReturnUrl { get; set; }
            public string Transport { get; set; }
            public string TransportData { get; set; }
            public string TransportProvider { get; set; }
            public string UserCode { get; set; }
            public DateTime VerifiedOnUtc { get; set; }
            public string VerificationUri { get; set; }
        }
        internal static AnonnymousCodeDbModel ToDbModel(AnonnymousCodeInfo info) => new()
        {
            Id = info.Id,
            AllowedVerificationRetries = info.AllowedRetries,
            AnonnymousCode = info.AnonnymousCode,
            AuthorizedScopes = info.AuthorizedScopes.ToDelimitedString(" "),
            ClientId = info.ClientId,
            Description = info.Description,
            ExpiresOnUtc = info.CreatedOnUtc.AddSeconds(info.Lifetime),
            IsOpenId = info.IsOpenId,
            Lifetime = info.Lifetime,
            RequestedScopes = info.RequestedScopes.ToDelimitedString(" "),
            ReturnUrl = info.ReturnUrl,
            Transport = info.Transport,
            TransportData = info.TransportData,
            TransportProvider = info.TransportProvider,
            VerificationUri = info.VerificationUri,
        };
        internal static AnonnymousCodeInfo FromDbModel(AnonnymousCodeDbModel model) => new()
        {
            Id = model.Id,
            ActivatedOnUtc = model.ActivatedOnUtc,
            AllowedRetries = model.AllowedVerificationRetries,
            AnonnymousCode = model.AnonnymousCode,
            AuthorizedScopes = model.AuthorizedScopes.FromDelimitedString(" "),
            ClientId = model.ClientId,
            CreatedOnUtc = model.CreatedOnUtc,
            Description = model.Description,
            IsOpenId = model.IsOpenId,
            Lifetime = model.Lifetime,
            RequestedScopes = model.RequestedScopes.FromDelimitedString(" "),
            ReturnUrl = model.ReturnUrl,
            Transport = model.Transport,
            TransportData = model.TransportData,
            TransportProvider = model.TransportProvider,
            VerifiedOnUtc = model.VerifiedOnUtc,
            VerificationRetryCounter = model.VerificationRetryCounter,
            VerificationUri = model.VerificationUri,
        };
        #endregion
    }
}
