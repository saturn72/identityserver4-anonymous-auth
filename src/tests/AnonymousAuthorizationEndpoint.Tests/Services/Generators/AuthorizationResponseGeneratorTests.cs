using IdentityServer4.Anonymous.Models;
using IdentityServer4.Anonymous.Services;
using IdentityServer4.Anonymous.Services.Generators;
using IdentityServer4.Anonymous.Stores;
using IdentityServer4.Anonymous.Transport;
using IdentityServer4.Anonymous.Validation;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.Anonymous.Tests.Services.Generators
{
    public class AuthorizationResponseGeneratorTests
    {
        [Fact]
        public async Task ProcessAsync_ThrowsOnNullResult()
        {
            var log = new Mock<ILogger<AuthorizationResponseGenerator>>();
            var g = new AuthorizationResponseGenerator(null, null, null, null, null, null, log.Object);
            await Should.ThrowAsync<ArgumentNullException>(() => g.ProcessAsync(default));
        }
        [Fact]
        public async Task ProcessAsync_ThrowsOnMisingClient()
        {
            var log = new Mock<ILogger<AuthorizationResponseGenerator>>();
            var g = new AuthorizationResponseGenerator(null, null, null, null, null, null, log.Object);
            var vr = new AuthorizationRequestValidationResult(new ValidatedAnonymousAuthorizationRequest());
            await Should.ThrowAsync<ArgumentNullException>(() => g.ProcessAsync(vr));
        }
        [Fact]
        public async Task ProcessAsync_SendsUserCode()
        {
            string vCode = "1234567890",
                uCode = "user-code";

            var ucg = new Mock<IUserCodeGenerator>();
            ucg.Setup(u => u.RetryLimit).Returns(1);
            ucg.Setup(u => u.GenerateAsync()).ReturnsAsync(uCode);

            var ucs = new Mock<IUserCodeService>();
            ucs.Setup(u => u.GetGenerator(It.IsAny<string>()))
                .ReturnsAsync(ucg.Object);

            var oData = new AnonymousAuthorizationOptions();
            var opt = new Mock<IOptions<AnonymousAuthorizationOptions>>();
            opt.Setup(o => o.Value).Returns(oData);

            var cs = new Mock<IAnonymousCodeStore>();
            var trn = new Mock<ITransporter>();
            trn.Setup(t => t.ShouldHandle).Returns(c => Task.FromResult(true));
            var clock = new Mock<ISystemClock>();
            var hgs = new Mock<IHandleGenerationService>();
            hgs.Setup(h => h.GenerateAsync(It.IsAny<int>())).ReturnsAsync(vCode);

            var log = new Mock<ILogger<AuthorizationResponseGenerator>>();
            var g = new AuthorizationResponseGenerator(ucs.Object, opt.Object, cs.Object, new[] { trn.Object }, clock.Object, hgs.Object, log.Object);
            var client = new Client
            {
                ClientId = "c-id"
            };
            var vReq = new ValidatedAnonymousAuthorizationRequest
            {
                Client = client,
                Description = "desc",
                Provider = "p",
                Transport = Constants.TransportTypes.Sms,
                TransportData = "td",
            };
            var vr = new AuthorizationRequestValidationResult(vReq);
            var r = await g.ProcessAsync(vr);

            r.VerificationCode.ShouldBe(vCode);
            r.VerificationUri.ShouldBe(oData.VerificationUri);
            r.VerificationUriComplete.ShouldBe($"{oData.VerificationUri.RemoveTrailingSlash()}?{Constants.UserInteraction.VerificationCode}={vCode}");
            r.Lifetime.ShouldBe(oData.DefaultLifetime);
            r.Interval.ShouldBe(oData.Interval);

            cs.Verify(c => c.StoreAnonymousCodeInfoAsync(
                It.Is<string>(c => c == vCode),
                It.Is<AnonymousCodeInfo>(i =>
                    i.AllowedRetries == oData.AllowedRetries &&
                    i.ClientId == client.ClientId &&
                    i.CreatedOnUtc == default &&
                    i.Description == vReq.Description &&
                    i.Lifetime == r.Lifetime &&
                    i.ReturnUrl == vReq.RedirectUrl &&
                    i.RequestedScopes == vReq.RequestedScopes &&
                    i.UserCode == uCode.Sha256() &&
                    i.Transport == vReq.Transport &&
                    i.VerificationCode == r.VerificationCode)),
                Times.Once);

            clock.Verify(c => c.UtcNow, Times.Once);
            trn.Verify(t => t.Transport(It.Is<UserCodeTransportContext>(c =>
                c.Transport == vReq.Transport &&
                c.Data == vReq.TransportData &&
                c.Body.Contains(uCode) &&
                c.Provider == vReq.Provider
                )),
                Times.Once);
        }
    }
}
