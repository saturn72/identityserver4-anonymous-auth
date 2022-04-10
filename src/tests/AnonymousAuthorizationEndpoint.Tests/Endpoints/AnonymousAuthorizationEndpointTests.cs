using IdentityServer4.Anonymous;
using IdentityServer4.Anonymous.Endpoints;
using IdentityServer4.Anonymous.Endpoints.Results;
using IdentityServer4.Anonymous.Events;
using IdentityServer4.Anonymous.Services.Generators;
using IdentityServer4.Anonymous.Validation;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Xunit;

namespace Identityserver4.Anonymous.Tests.Endpoints
{
    public class AnonymousAuthorizationEndpointTests
    {

        [Fact]
        public async Task ProcessAsync_NonPostMethod()
        {
            var log = new Mock<ILogger<AnonymousAuthorizationEndpoint>>();
            var endpoint = new AnonymousAuthorizationEndpoint(null, null, null, null, log.Object);
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "non-post";

            var res = await endpoint.ProcessAsync(ctx);
            res.ShouldBeOfType<TokenErrorResult>();
        }
        [Fact]
        public async Task ProcessAsync_NoFormContent()
        {
            var log = new Mock<ILogger<AnonymousAuthorizationEndpoint>>();
            var endpoint = new AnonymousAuthorizationEndpoint(null, null, null, null, log.Object);
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "post";
            var res = await endpoint.ProcessAsync(ctx);
            res.ShouldBeOfType<TokenErrorResult>();
        }
        [Theory]
        [MemberData(nameof(ProcessAsync_FailedToValidateClient_DATA))]
        public async Task ProcessAsync_FailedToValidateClient(ClientSecretValidationResult csvr)
        {
            var log = new Mock<ILogger<AnonymousAuthorizationEndpoint>>();
            var cv = new Mock<IClientSecretValidator>();
            cv.Setup(c => c.ValidateAsync(It.IsAny<HttpContext>())).ReturnsAsync(csvr);

            var endpoint = new AnonymousAuthorizationEndpoint(cv.Object, null, null, null, log.Object);
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "post";
            ctx.Request.ContentType = "application/x-www-form-urlencoded";
            var fields = new Dictionary<string, StringValues>();
            ctx.Request.Form = new FormCollection(fields);

            var res = await endpoint.ProcessAsync(ctx);
            res.ShouldBeOfType<TokenErrorResult>();
        }
        public static IEnumerable<object[]> ProcessAsync_FailedToValidateClient_DATA = new[]
        {
            new object[] {null},
            new object[] { new ClientSecretValidationResult()},
            new object[] {new ClientSecretValidationResult
            {
                Client = new IdentityServer4.Models.Client(),
                IsError = true,
                Error = "err",
            }},
        };
        [Fact]
        public async Task ProcessAsync_FailedToValidateForm()
        {
            var log = new Mock<ILogger<AnonymousAuthorizationEndpoint>>();
            var cv = new Mock<IClientSecretValidator>();
            var csvr = new ClientSecretValidationResult
            {
                Client = new IdentityServer4.Models.Client(),
                IsError = false,
            };
            cv.Setup(c => c.ValidateAsync(It.IsAny<HttpContext>())).ReturnsAsync(csvr);

            var rv = new Mock<IAnonymousAuthorizationRequestValidator>();
            var arvr = new AuthorizationRequestValidationResult(null, "err");
            rv.Setup(r => r.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClientSecretValidationResult>()))
                .ReturnsAsync(arvr);

            var evt = new Mock<IEventService>();

            var endpoint = new AnonymousAuthorizationEndpoint(cv.Object, rv.Object, null, evt.Object, log.Object);
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "post";
            ctx.Request.ContentType = "application/x-www-form-urlencoded";
            var fields = new Dictionary<string, StringValues>();
            ctx.Request.Form = new FormCollection(fields);

            var res = await endpoint.ProcessAsync(ctx);
            res.ShouldBeOfType<TokenErrorResult>();
            evt.Verify(e => e.RaiseAsync(It.IsAny<AnonymousAuthorizationFailureEvent>()), Times.Once);
        }
        [Fact]
        public async Task ProcessAsync_AuthorizeSuccess()
        {
            var log = new Mock<ILogger<AnonymousAuthorizationEndpoint>>();
            var cv = new Mock<IClientSecretValidator>();
            var csvr = new ClientSecretValidationResult
            {
                Client = new IdentityServer4.Models.Client(),
                IsError = false,
            };
            cv.Setup(c => c.ValidateAsync(It.IsAny<HttpContext>())).ReturnsAsync(csvr);

            var rv = new Mock<IAnonymousAuthorizationRequestValidator>();
            var arvr = new AuthorizationRequestValidationResult(null);
            rv.Setup(r => r.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClientSecretValidationResult>()))
                .ReturnsAsync(arvr);

            var evt = new Mock<IEventService>();
            var rg = new Mock<IAuthorizationResponseGenerator>();
            rg.Setup(r => r.ProcessAsync(It.IsAny<AuthorizationRequestValidationResult>()))
                .ReturnsAsync(new AuthorizationResponse());

            var endpoint = new AnonymousAuthorizationEndpoint(cv.Object, rv.Object, rg.Object, evt.Object, log.Object);
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "post";
            ctx.Request.ContentType = "application/x-www-form-urlencoded";
            var fields = new Dictionary<string, StringValues>();
            ctx.Request.Form = new FormCollection(fields);

            var res = await endpoint.ProcessAsync(ctx);

            res.ShouldBeOfType<AuthorizationResult>();
            evt.Verify(e => e.RaiseAsync(It.IsAny<AnonymousAuthorizationSuccessEvent>()), Times.Once);
            evt.Verify(e => e.RaiseAsync(It.IsAny<AnonymousAuthorizationFailureEvent>()), Times.Never);
        }
    }
}
