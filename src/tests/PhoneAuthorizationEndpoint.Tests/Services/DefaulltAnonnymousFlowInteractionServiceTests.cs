using IdentityServer4.Anonnymous.Services;
using IdentityServer4.Anonnymous.Services.Generators;
using IdentityServer4.Anonnymous.Stores;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Identityserver4.Anonnymous.Tests.Services
{
    public class DefaulltAnonnymousFlowInteractionServiceTests
    {
        [Fact]
        public async Task HandleRequestAsync_ThrowsOnDefaultCode()
        {
            var d = new DefaltAnonnymousFlowInteractionService(null, null, null, null, null);
            await Should.ThrowAsync<ArgumentNullException>(() => d.HandleRequestAsync(default));
        }
        [Fact]
        public async Task HandleRequestAsync_FailsOnNullClientId()
        {
            var clients = new Mock<IClientStore>();
            var logger = new Mock<ILogger<DefaltAnonnymousFlowInteractionService>>();
            var d = new DefaltAnonnymousFlowInteractionService(clients.Object, null, null, null, logger.Object);
            var code = new AnonnymousCodeInfo
            {
                ClientId = "cId"
            };
            var res = await d.HandleRequestAsync(code);
            res.IsError.ShouldBeTrue();
            res.ErrorDescription.ShouldNotBeNull();
        }

        [Fact]
        public async Task HandleRequestAsync_CreateSession()
        {
            var client = new Client
            {
                AllowedScopes = new[] { "scp-1", "scp-2" },
            };
            var cs = new Mock<IClientStore>();
            cs.Setup(c => c.FindClientByIdAsync(It.IsAny<string>())).ReturnsAsync(client);

            var acs = new Mock<IAnnonymousCodeStore>();
            var l = new Mock<ILogger<DefaltAnonnymousFlowInteractionService>>();

            var gn = new Mock<IRandomStringGenerator>();
            gn.Setup(g => g.Genetare(It.IsAny<int>())).ReturnsAsync("1234567890");

            var sessionId = "s-id";
            var us = new Mock<IUserSession>();
            us.Setup(u => u.CreateSessionIdAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .ReturnsAsync(sessionId);
            var d = new DefaltAnonnymousFlowInteractionService(cs.Object, us.Object, acs.Object, gn.Object, l.Object);
            var code = new AnonnymousCodeInfo
            {
                ClientId = "cId",
                RequestedScopes = new[] { "scp-1" }
            };
            var res = await d.HandleRequestAsync(code);
            res.IsError.ShouldBeFalse();
            res.ErrorDescription.ShouldBeNull();

            gn.Verify(g => g.Genetare(It.IsAny<int>()), Times.Once);
        }
        [Fact]
        public async Task HandleRequestAsync_CreateSession_GenerateMultipleTimes()
        {
            var client = new Client
            {
                AllowedScopes = new[] { "scp-1", "scp-2" },
            };
            var cs = new Mock<IClientStore>();
            cs.Setup(c => c.FindClientByIdAsync(It.IsAny<string>())).ReturnsAsync(client);

            var gen = new[] { "1234567890", "co" };
            var acs = new Mock<IAnnonymousCodeStore>();
            acs.Setup(s => s.GetAllSubjectIds()).ReturnsAsync(new[] { gen[0] });
            var l = new Mock<ILogger<DefaltAnonnymousFlowInteractionService>>();

            var gn = new Mock<IRandomStringGenerator>();
            var i = 0;
            gn.Setup(g => g.Genetare(It.IsAny<int>())).ReturnsAsync(() => gen[i++]);

            var sessionId = "s-id";
            var us = new Mock<IUserSession>();
            us.Setup(u => u.CreateSessionIdAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .ReturnsAsync(sessionId);

            var d = new DefaltAnonnymousFlowInteractionService(cs.Object, us.Object, acs.Object, gn.Object, l.Object);
            var code = new AnonnymousCodeInfo
            {
                ClientId = "cId",
                RequestedScopes = new[] { "scp-1" }
            };
            var res = await d.HandleRequestAsync(code);
            res.IsError.ShouldBeFalse();
            res.ErrorDescription.ShouldBeNull();
            gn.Verify(g => g.Genetare(It.IsAny<int>()), Times.Exactly(2));
        }
    }
}
