using IdentityServer4.Anonnymous.Models;
using IdentityServer4.Anonnymous.Services;
using IdentityServer4.Anonnymous.Services.Generators;
using IdentityServer4.Anonnymous.Stores;
using IdentityServer4.Anonnymous.Transport;
using IdentityServer4.Anonnymous.Validation;
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

namespace IdentityServer4.Anonnymous.Tests.Services.Generators
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
            var vr = new AuthorizationRequestValidationResult(new ValidatedAnonnymousAuthorizationRequest());
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

            var oData = new AnonnymousAuthorizationOptions();
            var opt = new Mock<IOptions<AnonnymousAuthorizationOptions>>();
            opt.Setup(o => o.Value).Returns(oData);

            var cs = new Mock<IAnnonymousCodeStore>();
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
            var vReq = new ValidatedAnonnymousAuthorizationRequest
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

            cs.Verify(c => c.StoreAnonnymousCodeInfoAsync(
                It.Is<string>(c => c == vCode),
                It.Is<AnonnymousCodeInfo>(i =>
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
