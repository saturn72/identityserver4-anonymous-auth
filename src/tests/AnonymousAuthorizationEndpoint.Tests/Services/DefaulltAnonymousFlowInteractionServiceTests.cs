using IdentityServer4.Anonymous.Services;
using IdentityServer4.Anonymous.Services.Generators;
using IdentityServer4.Anonymous.Stores;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Identityserver4.Anonymous.Tests.Services
{
    public class DefaulltAnonymousFlowInteractionServiceTests
    {
        [Fact]
        public async Task HandleRequestAsync_ThrowsOnDefaultCode()
        {
            var d = new DefaltAnonymousFlowInteractionService(null, null, null, null, null);
            await Should.ThrowAsync<ArgumentNullException>(() => d.HandleRequestAsync(default));
        }
        [Fact]
        public async Task HandleRequestAsync_FailsOnNullClientId()
        {
            var clients = new Mock<IClientStore>();
            var logger = new Mock<ILogger<DefaltAnonymousFlowInteractionService>>();
            var d = new DefaltAnonymousFlowInteractionService(clients.Object, null, null, null, logger.Object);
            var code = new AnonymousCodeInfo
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

            var acs = new Mock<IAnonymousCodeStore>();
            var l = new Mock<ILogger<DefaltAnonymousFlowInteractionService>>();

            var gn = new Mock<IRandomStringGenerator>();
            gn.Setup(g => g.Genetare(It.IsAny<int>())).ReturnsAsync("1234567890");

            var sessionId = "s-id";
            var us = new Mock<IUserSession>();
            us.Setup(u => u.CreateSessionIdAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .ReturnsAsync(sessionId);
            var d = new DefaltAnonymousFlowInteractionService(cs.Object, us.Object, acs.Object, gn.Object, l.Object);
            var code = new AnonymousCodeInfo
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
        public async Task HandleRequestAsync_CreateSession_NeverGenerates()
        {
            var client = new Client
            {
                AllowedScopes = new[] { "scp-1", "scp-2" },
            };
            var cs = new Mock<IClientStore>();
            cs.Setup(c => c.FindClientByIdAsync(It.IsAny<string>())).ReturnsAsync(client);

            var acs = new Mock<IAnonymousCodeStore>();
            acs.Setup(s => s.GetAllSubjectIdsByClientIdAsync(It.IsAny<string>())).ReturnsAsync(new[] { "1", "2", "3" });
            var l = new Mock<ILogger<DefaltAnonymousFlowInteractionService>>();
            var gn = new Mock<IRandomStringGenerator>();

            var sessionId = "s-id";
            var us = new Mock<IUserSession>();
            us.Setup(u => u.CreateSessionIdAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .ReturnsAsync(sessionId);

            var d = new DefaltAnonymousFlowInteractionService(cs.Object, us.Object, acs.Object, gn.Object, l.Object);
            var code = new AnonymousCodeInfo
            {
                Id = Guid.NewGuid(),
                ClientId = "cId",
                RequestedScopes = new[] { "scp-1" }
            };
            var res = await d.HandleRequestAsync(code);
            res.IsError.ShouldBeFalse();
            res.ErrorDescription.ShouldBeNull();
            gn.Verify(g => g.Genetare(It.IsAny<int>()), Times.Never);
        }
        [Theory]
        [MemberData(nameof(HandleRequestAsync_CreateSession_GenerateMultipleTimes_DATA))]
        public async Task HandleRequestAsync_CreateSession_GenerateMultipleTimes(string[] subjects)
        {
            var client = new Client
            {
                AllowedScopes = new[] { "scp-1", "scp-2" },
            };
            var cs = new Mock<IClientStore>();
            cs.Setup(c => c.FindClientByIdAsync(It.IsAny<string>())).ReturnsAsync(client);

            var acs = new Mock<IAnonymousCodeStore>();
            acs.Setup(s => s.GetAllSubjectIdsByClientIdAsync(It.IsAny<string>())).ReturnsAsync(subjects[0..^1]);
            var l = new Mock<ILogger<DefaltAnonymousFlowInteractionService>>();

            var gn = new Mock<IRandomStringGenerator>();
            var i = 0;
            gn.Setup(g => g.Genetare(It.IsAny<int>())).ReturnsAsync(() => subjects[i++]);

            var sessionId = "s-id";
            var us = new Mock<IUserSession>();
            us.Setup(u => u.CreateSessionIdAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .ReturnsAsync(sessionId);

            var d = new DefaltAnonymousFlowInteractionService(cs.Object, us.Object, acs.Object, gn.Object, l.Object);
            var code = new AnonymousCodeInfo
            {
                ClientId = "cId",
                RequestedScopes = new[] { "scp-1" }
            };
            var res = await d.HandleRequestAsync(code);
            res.IsError.ShouldBeFalse();
            res.ErrorDescription.ShouldBeNull();
            gn.Verify(g => g.Genetare(It.IsAny<int>()), Times.Exactly(subjects.Length));
        }
        public static IEnumerable<object[]> HandleRequestAsync_CreateSession_GenerateMultipleTimes_DATA = new[]
        {
            new object[]{new[] { "co" } },
            new object[]{new[] { "1", "2", "co" } },
            new object[]{new[] { "1", "2", "3", "co" } },
        };
    }
}
