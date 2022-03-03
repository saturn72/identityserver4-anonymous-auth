using IdentityServer4.Anonymous.Services;
using static IdentityServer4.Anonymous.Stores.DefaultAnonymousCodeStore;

namespace IdentityServer4.Anonymous.Stores
{
    public sealed class SqlScripts
    {
        public sealed class AnonymousCodeScripts
        {
            private static readonly string TableName = $"{nameof(AnonymousCodeInfo)}s";
            private static readonly string SelectSingleBase = $"SELECT " +
                $"[{nameof(AnonymousCodeDbModel.Id)}], " +
                $"[{nameof(AnonymousCodeDbModel.AllowedRetries)}], " +
                $"[{nameof(AnonymousCodeDbModel.AuthorizedScopes)}], " +
                $"[{nameof(AnonymousCodeDbModel.ClientId)}], " +
                $"[{nameof(AnonymousCodeDbModel.Description)}], " +
                $"[{nameof(AnonymousCodeDbModel.RequestedScopes)}], " +
                $"[{nameof(AnonymousCodeDbModel.ReturnUrl)}], " +
                $"[{nameof(AnonymousCodeDbModel.Transport)}], " +
                $"[{nameof(AnonymousCodeDbModel.VerificationCode)}] " +
                $"FROM [{TableName}]";

            public static readonly string SelectByUserCode = SelectSingleBase + $" WHERE [{nameof(AnonymousCodeDbModel.UserCode)}] = @{nameof(AnonymousCodeDbModel.UserCode)}";

            public static readonly string SelectByUserCodeExcludeExpiredAndVerified = SelectByUserCode +
                $" AND [{nameof(AnonymousCodeDbModel.ExpiresOnUtc)}] > SYSUTCDATETIME()" +
                $" AND [{nameof(AnonymousCodeDbModel.VerifiedOnUtc)}] IS NULL" +
                $" AND [{nameof(AnonymousCodeDbModel.RetryCounter)}] < [{nameof(AnonymousCodeDbModel.AllowedRetries)}]";

            public static readonly string SelectByVerificationCodeAndUserCode =
                SelectSingleBase + $" WHERE [{nameof(AnonymousCodeDbModel.UserCode)}] = @{nameof(AnonymousCodeDbModel.UserCode)}" +
                $" AND [{nameof(AnonymousCodeDbModel.VerificationCode)}] = @{nameof(AnonymousCodeDbModel.VerificationCode)}";

            public static readonly string SelectByVerificationAndUserCodeExcludeExpiredAndVerified = SelectByVerificationCodeAndUserCode +
                $" AND [{nameof(AnonymousCodeDbModel.ExpiresOnUtc)}] > SYSUTCDATETIME()" +
                $" AND [{nameof(AnonymousCodeDbModel.VerifiedOnUtc)}] IS NULL" +
                $" AND [{nameof(AnonymousCodeDbModel.RetryCounter)}] < [{nameof(AnonymousCodeDbModel.AllowedRetries)}]";

            public static readonly string SelectByVeridicationCode = SelectSingleBase + $" WHERE [{nameof(AnonymousCodeDbModel.VerificationCode)}] = @{nameof(AnonymousCodeDbModel.VerificationCode)}";

            public static readonly string SelectByVerificationCodeExcludeExpiredAndVerified = SelectByVeridicationCode +
                $" AND [{nameof(AnonymousCodeDbModel.ExpiresOnUtc)}] > SYSUTCDATETIME()" +
                $" AND [{nameof(AnonymousCodeDbModel.IsAuthorized)}] = 0" +
                $" AND [{nameof(AnonymousCodeDbModel.VerifiedOnUtc)}] IS NULL" +
                $" AND [{nameof(AnonymousCodeDbModel.RetryCounter)}] < [{nameof(AnonymousCodeDbModel.AllowedRetries)}]";


            public static readonly string InsertCommand = $"INSERT INTO [{TableName}] (" +
                $"[{nameof(AnonymousCodeDbModel.AllowedRetries)}], " +
                $"[{nameof(AnonymousCodeDbModel.VerificationCode)}], " +
                $"{nameof(AnonymousCodeDbModel.ClientId)}, " +
                $"{nameof(AnonymousCodeDbModel.Description)}, " +
                $"{nameof(AnonymousCodeDbModel.ExpiresOnUtc)}, " +
                $"{nameof(AnonymousCodeDbModel.Lifetime)}, " +
                $"{nameof(AnonymousCodeDbModel.RequestedScopes)}, " +
                $"{nameof(AnonymousCodeDbModel.ReturnUrl)}, " +
                $"{nameof(AnonymousCodeDbModel.Transport)}, " +
                $"{nameof(AnonymousCodeDbModel.UserCode)}" +
                $") VALUES (" +
                $"@{nameof(AnonymousCodeDbModel.AllowedRetries)}, " +
                $"@{nameof(AnonymousCodeDbModel.VerificationCode)}, " +
                $"@{nameof(AnonymousCodeDbModel.ClientId)}, " +
                $"@{nameof(AnonymousCodeDbModel.Description)}, " +
                $"@{nameof(AnonymousCodeDbModel.ExpiresOnUtc)}, " +
                $"@{nameof(AnonymousCodeDbModel.Lifetime)}, " +
                $"@{nameof(AnonymousCodeDbModel.RequestedScopes)}, " +
                $"@{nameof(AnonymousCodeDbModel.ReturnUrl)}, " +
                $"@{nameof(AnonymousCodeDbModel.Transport)}, " +
                $"@{nameof(AnonymousCodeDbModel.UserCode)})";

            public static readonly string UpdateVerificationRetry = $"UPDATE [{TableName}] SET " +
                $"[{nameof(AnonymousCodeDbModel.RetryCounter)}] = [{nameof(AnonymousCodeDbModel.RetryCounter)}] + 1 " +
                $"WHERE [{nameof(AnonymousCodeDbModel.VerificationCode)}] = @{nameof(AnonymousCodeDbModel.VerificationCode)}";

            public static readonly string SelectSubjectByClientId = $"SELECT [{nameof(AnonymousCodeDbModel.Subject)}] FROM [{TableName}]" +
                $" WHERE [{nameof(AnonymousCodeDbModel.ClientId)}] = @{nameof(AnonymousCodeDbModel.ClientId)} AND [{nameof(AnonymousCodeDbModel.Subject)}] != NULL";

            public static readonly string UpdateAuthorization = $"UPDATE [{TableName}] SET " +
                $"[{nameof(AnonymousCodeDbModel.Claims)}] = @{nameof(AnonymousCodeDbModel.Claims)}, " +
                $"[{nameof(AnonymousCodeDbModel.IsAuthorized)}] = 1, " +
                $"[{nameof(AnonymousCodeDbModel.Subject)}] = @{nameof(AnonymousCodeDbModel.Subject)} " +
                $"WHERE [{nameof(AnonymousCodeDbModel.VerificationCode)}] = @{nameof(AnonymousCodeDbModel.VerificationCode)}";
        }
    }
}
