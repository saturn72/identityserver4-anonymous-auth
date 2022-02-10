using IdentityServer4.Anonnymous.Services;
using static IdentityServer4.Anonnymous.Data.DapperAnnonymousCodeStore;

namespace IdentityServer4.Anonnymous.Data
{
    public sealed class SqlScripts
    {
        public sealed class AnonnymousCodeScripts
        {
            private static readonly string TableName = $"{nameof(AnonnymousCodeInfo)}s";
            public static readonly string SelectSingleBase = $"SELECT " +
                $"[{nameof(AnonnymousCodeDbModel.Id)}], " +
                $"[{nameof(AnonnymousCodeDbModel.ActivatedOnUtc)}], " +
                $"[{nameof(AnonnymousCodeDbModel.AllowedVerificationRetries)}], " +
                $"[{nameof(AnonnymousCodeDbModel.AuthorizedScopes)}], " +
                $"[{nameof(AnonnymousCodeDbModel.AnonnymousCode)}], " +
                $"[{nameof(AnonnymousCodeDbModel.ClientId)}], " +
                $"[{nameof(AnonnymousCodeDbModel.CreatedOnUtc)}], " +
                $"[{nameof(AnonnymousCodeDbModel.Description)}], " +
                $"[{nameof(AnonnymousCodeDbModel.IsOpenId)}], " +
                $"[{nameof(AnonnymousCodeDbModel.Lifetime)}], " +
                $"[{nameof(AnonnymousCodeDbModel.RequestedScopes)}], " +
                $"[{nameof(AnonnymousCodeDbModel.ReturnUrl)}], " +
                $"[{nameof(AnonnymousCodeDbModel.Transport)}], " +
                $"[{nameof(AnonnymousCodeDbModel.TransportData)}], " +
                $"[{nameof(AnonnymousCodeDbModel.TransportProvider)}], " +
                $"[{nameof(AnonnymousCodeDbModel.VerificationRetryCounter)}], " +
                $"[{nameof(AnonnymousCodeDbModel.VerifiedOnUtc)}] " +
                $"FROM [{TableName}]";

            public static readonly string SelectByUserCode = SelectSingleBase + $"WHERE [{nameof(AnonnymousCodeDbModel.UserCode)}] = @{nameof(AnonnymousCodeDbModel.UserCode)}";

            public static readonly string SelectByUserCodeExcludeExpiredActivatedAndVerified = SelectByUserCode +
                $" AND [{nameof(AnonnymousCodeDbModel.ExpiresOnUtc)}] > SYSUTCDATETIME()" +
                $" AND [{nameof(AnonnymousCodeDbModel.VerifiedOnUtc)}] IS NULL" +
                $" AND (([{nameof(AnonnymousCodeDbModel.ActivatedOnUtc)}] IS NULL)" +
                    $" OR ([{nameof(AnonnymousCodeDbModel.ActivatedOnUtc)}] IS NOT NULL AND [{nameof(AnonnymousCodeDbModel.AllowedVerificationRetries)}] <= [{nameof(AnonnymousCodeDbModel.VerificationRetryCounter)}]))";

            public static readonly string SelectByAnonnymousCode = SelectSingleBase + $"WHERE [{nameof(AnonnymousCodeDbModel.AnonnymousCode)}] = @{nameof(AnonnymousCodeDbModel.AnonnymousCode)}";

            public static readonly string SelectByAnonnymousCodeExcludeExpiredActivatedAndVerified = SelectByAnonnymousCode +
                $" AND [{nameof(AnonnymousCodeDbModel.ExpiresOnUtc)}] > SYSUTCDATETIME()" +
                $" AND [{nameof(AnonnymousCodeDbModel.VerifiedOnUtc)}] IS NULL" +
                $" AND (([{nameof(AnonnymousCodeDbModel.ActivatedOnUtc)}] IS NULL)" +
                    $" OR ([{nameof(AnonnymousCodeDbModel.ActivatedOnUtc)}] IS NOT NULL AND [{nameof(AnonnymousCodeDbModel.AllowedVerificationRetries)}] <= [{nameof(AnonnymousCodeDbModel.VerificationRetryCounter)}]))";


            public static readonly string InsertCommand = $"INSERT INTO [{TableName}] (" +
                $"{nameof(AnonnymousCodeDbModel.AuthorizedScopes)}, " +
                $"[{nameof(AnonnymousCodeDbModel.AnonnymousCode)}], " +
                $"{nameof(AnonnymousCodeDbModel.ClientId)}, " +
                $"{nameof(AnonnymousCodeDbModel.Description)}, " +
                $"{nameof(AnonnymousCodeDbModel.ExpiresOnUtc)}, " +
                $"{nameof(AnonnymousCodeDbModel.IsOpenId)}, " +
                $"{nameof(AnonnymousCodeDbModel.Lifetime)}, " +
                $"{nameof(AnonnymousCodeDbModel.RequestedScopes)}, " +
                $"{nameof(AnonnymousCodeDbModel.ReturnUrl)}, " +
                $"{nameof(AnonnymousCodeDbModel.Transport)}, " +
                $"{nameof(AnonnymousCodeDbModel.TransportData)}, " +
                $"{nameof(AnonnymousCodeDbModel.TransportProvider)}" +
                $") VALUES (" +
                $"@{nameof(AnonnymousCodeDbModel.AuthorizedScopes)}, " +
                $"@{nameof(AnonnymousCodeDbModel.AnonnymousCode)}, " +
                $"@{nameof(AnonnymousCodeDbModel.ClientId)}, " +
                $"@{nameof(AnonnymousCodeDbModel.Description)}, " +
                $"@{nameof(AnonnymousCodeDbModel.ExpiresOnUtc)}, " +
                $"@{nameof(AnonnymousCodeDbModel.IsOpenId)}, " +
                $"@{nameof(AnonnymousCodeDbModel.Lifetime)}, " +
                $"@{nameof(AnonnymousCodeDbModel.RequestedScopes)}, " +
                $"@{nameof(AnonnymousCodeDbModel.ReturnUrl)}, " +
                $"@{nameof(AnonnymousCodeDbModel.Transport)}, " +
                $"@{nameof(AnonnymousCodeDbModel.TransportData)}, " +
                $"@{nameof(AnonnymousCodeDbModel.TransportProvider)})";

            public static readonly string Activate = $"UPDATE [{TableName}] SET " +
                $"[{nameof(AnonnymousCodeDbModel.ActivatedOnUtc)}] = SYSUTCDATETIME(), " +
                $"[{nameof(AnonnymousCodeDbModel.UserCode)}] = @{nameof(AnonnymousCodeDbModel.UserCode)} " +
                $"WHERE [{nameof(AnonnymousCodeDbModel.Id)}] = @{nameof(AnonnymousCodeDbModel.Id)}";
        }
    }
}
