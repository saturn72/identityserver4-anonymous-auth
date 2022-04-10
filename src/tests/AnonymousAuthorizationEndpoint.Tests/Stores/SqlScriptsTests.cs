
using IdentityServer4.Anonymous.Stores;
using Shouldly;
using Xunit;

namespace IdentityServer4.Anonymous.Tests.Stores
{
    public class SqlScriptsTests
    {
        public class AnonymousCodeScriptsTests
        {
            [Fact]
            public void ALL()
            {
                SqlScripts.AnonymousCodeScripts.SelectByUserCode.ShouldBe("SELECT [Id], [AllowedRetries], [AuthorizedScopes], [ClientId], [Description], [RequestedScopes], [ReturnUrl], [Transport] FROM [AnonymousCodeInfos] WHERE [UserCode] = @UserCode");
                SqlScripts.AnonymousCodeScripts.SelectByUserCodeExcludeExpiredAndVerified.ShouldBe("SELECT [Id], [AllowedRetries], [AuthorizedScopes], [ClientId], [Description], [RequestedScopes], [ReturnUrl], [Transport] FROM [AnonymousCodeInfos] WHERE [UserCode] = @UserCode AND [ExpiresOnUtc] > SYSUTCDATETIME() AND [VerifiedOnUtc] IS NULL AND [RetryCounter] < [AllowedRetries]");
                SqlScripts.AnonymousCodeScripts.SelectByVerificationCodeAndUserCode.ShouldBe("SELECT [Id], [AllowedRetries], [AuthorizedScopes], [ClientId], [Description], [RequestedScopes], [ReturnUrl], [Transport] FROM [AnonymousCodeInfos] WHERE [UserCode] = @UserCode AND [VerificationCode] = @VerificationCode");
                SqlScripts.AnonymousCodeScripts.SelectByVerificationAndUserCodeExcludeExpiredAndVerified.ShouldBe("SELECT [Id], [AllowedRetries], [AuthorizedScopes], [ClientId], [Description], [RequestedScopes], [ReturnUrl], [Transport] FROM [AnonymousCodeInfos] WHERE [UserCode] = @UserCode AND [VerificationCode] = @VerificationCode AND [ExpiresOnUtc] > SYSUTCDATETIME() AND [VerifiedOnUtc] IS NULL AND [RetryCounter] < [AllowedRetries]");
                SqlScripts.AnonymousCodeScripts.SelectByVeridicationCode.ShouldBe("SELECT [Id], [AllowedRetries], [AuthorizedScopes], [ClientId], [Description], [RequestedScopes], [ReturnUrl], [Transport] FROM [AnonymousCodeInfos] WHERE [VerificationCode] = @VerificationCode");
                SqlScripts.AnonymousCodeScripts.SelectByVerificationCodeExcludeExpiredAndVerified.ShouldBe("SELECT [Id], [AllowedRetries], [AuthorizedScopes], [ClientId], [Description], [RequestedScopes], [ReturnUrl], [Transport] FROM [AnonymousCodeInfos] WHERE [VerificationCode] = @VerificationCode AND [ExpiresOnUtc] > SYSUTCDATETIME() AND [IsAuthorized] = 0 AND [VerifiedOnUtc] IS NULL AND [RetryCounter] < [AllowedRetries]");
                SqlScripts.AnonymousCodeScripts.InsertCommand.ShouldBe("INSERT INTO [AnonymousCodeInfos] ([AllowedRetries], [VerificationCode], ClientId, Description, ExpiresOnUtc, Lifetime, RequestedScopes, ReturnUrl, Transport, UserCode) VALUES (@AllowedRetries, @VerificationCode, @ClientId, @Description, @ExpiresOnUtc, @Lifetime, @RequestedScopes, @ReturnUrl, @Transport, @UserCode)");
                SqlScripts.AnonymousCodeScripts.UpdateVerificationRetry.ShouldBe("UPDATE [AnonymousCodeInfos] SET [RetryCounter] = [RetryCounter] + 1 WHERE [VerificationCode] = @VerificationCode");

                SqlScripts.AnonymousCodeScripts.SelectSubjectByClientId.ShouldBe("SELECT [Subject] FROM [AnonymousCodeInfos] WHERE [ClientId] = @ClientId AND [Subject] != NULL");

                SqlScripts.AnonymousCodeScripts.UpdateAuthorization.ShouldBe("UPDATE [AnonymousCodeInfos] SET [Claims] = @[Claims], [IsAuthorized] = 1, [Subject] = @[Subject] WHERE [VerificationCode] = @VerificationCode");

            }
        }
    }
}
