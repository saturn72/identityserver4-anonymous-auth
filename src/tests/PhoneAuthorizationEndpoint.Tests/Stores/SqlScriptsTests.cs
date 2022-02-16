
using IdentityServer4.Anonnymous.Stores;
using Shouldly;
using Xunit;

namespace IdentityServer4.Anonnymous.Tests.Stores
{
    public class SqlScriptsTests
    {
        public class AnonnymousCodeScriptsTests
        {
            [Fact]
            public void ALL()
            {
                SqlScripts.AnonnymousCodeScripts.SelectByUserCode.ShouldBe("SELECT [Id], [AllowedRetries], [AuthorizedScopes], [ClientId], [Description], [RequestedScopes], [ReturnUrl], [Transport] FROM [AnonnymousCodeInfos] WHERE [UserCode] = @UserCode");
                SqlScripts.AnonnymousCodeScripts.SelectByUserCodeExcludeExpiredAndVerified.ShouldBe("SELECT [Id], [AllowedRetries], [AuthorizedScopes], [ClientId], [Description], [RequestedScopes], [ReturnUrl], [Transport] FROM [AnonnymousCodeInfos] WHERE [UserCode] = @UserCode AND [ExpiresOnUtc] > SYSUTCDATETIME() AND [VerifiedOnUtc] IS NULL AND [RetryCounter] < [AllowedRetries]");
                SqlScripts.AnonnymousCodeScripts.SelectByVerificationCodeAndUserCode.ShouldBe("SELECT [Id], [AllowedRetries], [AuthorizedScopes], [ClientId], [Description], [RequestedScopes], [ReturnUrl], [Transport] FROM [AnonnymousCodeInfos] WHERE [UserCode] = @UserCode AND [VerificationCode] = @VerificationCode");
                SqlScripts.AnonnymousCodeScripts.SelectByVerificationAndUserCodeExcludeExpiredAndVerified.ShouldBe("SELECT [Id], [AllowedRetries], [AuthorizedScopes], [ClientId], [Description], [RequestedScopes], [ReturnUrl], [Transport] FROM [AnonnymousCodeInfos] WHERE [UserCode] = @UserCode AND [VerificationCode] = @VerificationCode AND [ExpiresOnUtc] > SYSUTCDATETIME() AND [VerifiedOnUtc] IS NULL AND [RetryCounter] < [AllowedRetries]");
                SqlScripts.AnonnymousCodeScripts.SelectByVeridicationCode.ShouldBe("SELECT [Id], [AllowedRetries], [AuthorizedScopes], [ClientId], [Description], [RequestedScopes], [ReturnUrl], [Transport] FROM [AnonnymousCodeInfos] WHERE [VerificationCode] = @VerificationCode");
                SqlScripts.AnonnymousCodeScripts.SelectByVerificationCodeExcludeExpiredAndVerified.ShouldBe("SELECT [Id], [AllowedRetries], [AuthorizedScopes], [ClientId], [Description], [RequestedScopes], [ReturnUrl], [Transport] FROM [AnonnymousCodeInfos] WHERE [VerificationCode] = @VerificationCode AND [ExpiresOnUtc] > SYSUTCDATETIME() AND [VerifiedOnUtc] IS NULL AND [RetryCounter] < [AllowedRetries]");
                SqlScripts.AnonnymousCodeScripts.InsertCommand.ShouldBe("INSERT INTO [AnonnymousCodeInfos] ([AllowedRetries], [VerificationCode], ClientId, Description, ExpiresOnUtc, Lifetime, RequestedScopes, ReturnUrl, Transport, UserCode) VALUES (@AllowedRetries, @VerificationCode, @ClientId, @Description, @ExpiresOnUtc, @Lifetime, @RequestedScopes, @ReturnUrl, @Transport, @UserCode)");
                SqlScripts.AnonnymousCodeScripts.UpdateVerificationRetry.ShouldBe("UPDATE [AnonnymousCodeInfos] SET [RetryCounter] = [RetryCounter] + 1 WHERE [VerificationCode] = @VerificationCode");
            }
        }
    }
}
