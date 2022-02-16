using IdentityServer4.Anonnymous.Services;
using static IdentityServer4.Anonnymous.Stores.DefaultAnnonymousCodeStore;

namespace IdentityServer4.Anonnymous.Stores
{
    public sealed class SqlScripts
    {
        public sealed class AnonnymousCodeScripts
        {
            private static readonly string TableName = $"{nameof(AnonnymousCodeInfo)}s";
            private static readonly string SelectSingleBase = $"SELECT " +
                $"[{nameof(AnonnymousCodeDbModel.Id)}], " +
                $"[{nameof(AnonnymousCodeDbModel.AllowedRetries)}], " +
                $"[{nameof(AnonnymousCodeDbModel.AuthorizedScopes)}], " +
                $"[{nameof(AnonnymousCodeDbModel.ClientId)}], " +
                $"[{nameof(AnonnymousCodeDbModel.Description)}], " +
                $"[{nameof(AnonnymousCodeDbModel.RequestedScopes)}], " +
                $"[{nameof(AnonnymousCodeDbModel.ReturnUrl)}], " +
                $"[{nameof(AnonnymousCodeDbModel.Transport)}] " +
                $"FROM [{TableName}]";

            public static readonly string SelectByUserCode = SelectSingleBase + $" WHERE [{nameof(AnonnymousCodeDbModel.UserCode)}] = @{nameof(AnonnymousCodeDbModel.UserCode)}";

            public static readonly string SelectByUserCodeExcludeExpiredAndVerified = SelectByUserCode +
                $" AND [{nameof(AnonnymousCodeDbModel.ExpiresOnUtc)}] > SYSUTCDATETIME()" +
                $" AND [{nameof(AnonnymousCodeDbModel.VerifiedOnUtc)}] IS NULL" +
                $" AND [{nameof(AnonnymousCodeDbModel.RetryCounter)}] < [{nameof(AnonnymousCodeDbModel.AllowedRetries)}]";

            public static readonly string SelectByVerificationCodeAndUserCode =
                SelectSingleBase + $" WHERE [{nameof(AnonnymousCodeDbModel.UserCode)}] = @{nameof(AnonnymousCodeDbModel.UserCode)}" +
                $" AND [{nameof(AnonnymousCodeDbModel.VerificationCode)}] = @{nameof(AnonnymousCodeDbModel.VerificationCode)}";

            public static readonly string SelectByVerificationAndUserCodeExcludeExpiredAndVerified = SelectByVerificationCodeAndUserCode +
                $" AND [{nameof(AnonnymousCodeDbModel.ExpiresOnUtc)}] > SYSUTCDATETIME()" +
                $" AND [{nameof(AnonnymousCodeDbModel.VerifiedOnUtc)}] IS NULL" +
                $" AND [{nameof(AnonnymousCodeDbModel.RetryCounter)}] < [{nameof(AnonnymousCodeDbModel.AllowedRetries)}]";

            public static readonly string SelectByVeridicationCode = SelectSingleBase + $" WHERE [{nameof(AnonnymousCodeDbModel.VerificationCode)}] = @{nameof(AnonnymousCodeDbModel.VerificationCode)}";

            public static readonly string SelectByVerificationCodeExcludeExpiredAndVerified = SelectByVeridicationCode +
                $" AND [{nameof(AnonnymousCodeDbModel.ExpiresOnUtc)}] > SYSUTCDATETIME()" +
                $" AND [{nameof(AnonnymousCodeDbModel.VerifiedOnUtc)}] IS NULL" +
                $" AND [{nameof(AnonnymousCodeDbModel.RetryCounter)}] < [{nameof(AnonnymousCodeDbModel.AllowedRetries)}]";


            public static readonly string InsertCommand = $"INSERT INTO [{TableName}] (" +
                $"[{nameof(AnonnymousCodeDbModel.AllowedRetries)}], " +
                $"[{nameof(AnonnymousCodeDbModel.VerificationCode)}], " +
                $"{nameof(AnonnymousCodeDbModel.ClientId)}, " +
                $"{nameof(AnonnymousCodeDbModel.Description)}, " +
                $"{nameof(AnonnymousCodeDbModel.ExpiresOnUtc)}, " +
                $"{nameof(AnonnymousCodeDbModel.Lifetime)}, " +
                $"{nameof(AnonnymousCodeDbModel.RequestedScopes)}, " +
                $"{nameof(AnonnymousCodeDbModel.ReturnUrl)}, " +
                $"{nameof(AnonnymousCodeDbModel.Transport)}, " +
                $"{nameof(AnonnymousCodeDbModel.UserCode)}" +
                $") VALUES (" +
                $"@{nameof(AnonnymousCodeDbModel.AllowedRetries)}, " +
                $"@{nameof(AnonnymousCodeDbModel.VerificationCode)}, " +
                $"@{nameof(AnonnymousCodeDbModel.ClientId)}, " +
                $"@{nameof(AnonnymousCodeDbModel.Description)}, " +
                $"@{nameof(AnonnymousCodeDbModel.ExpiresOnUtc)}, " +
                $"@{nameof(AnonnymousCodeDbModel.Lifetime)}, " +
                $"@{nameof(AnonnymousCodeDbModel.RequestedScopes)}, " +
                $"@{nameof(AnonnymousCodeDbModel.ReturnUrl)}, " +
                $"@{nameof(AnonnymousCodeDbModel.Transport)}, " +
                $"@{nameof(AnonnymousCodeDbModel.UserCode)})";

            public static readonly string UpdateVerificationRetry = $"UPDATE [{TableName}] SET " +
                $"[{nameof(AnonnymousCodeDbModel.RetryCounter)}] = [{nameof(AnonnymousCodeDbModel.RetryCounter)}] + 1 " +
                $"WHERE [{nameof(AnonnymousCodeDbModel.VerificationCode)}] = @{nameof(AnonnymousCodeDbModel.VerificationCode)}";
        }
    }
}
