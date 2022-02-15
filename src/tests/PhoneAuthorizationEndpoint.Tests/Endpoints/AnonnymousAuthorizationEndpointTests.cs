using IdentityServer4.Anonnymous;
using IdentityServer4.Anonnymous.Endpoints;
using IdentityServer4.Anonnymous.Endpoints.Results;
using IdentityServer4.Anonnymous.Events;
using IdentityServer4.Anonnymous.ResponseHandling;
using IdentityServer4.Anonnymous.Validation;
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

namespace PhoneAuthorizationEndpoint.Tests.Endpoints
{
    public class AnonnymousAuthorizationEndpointTests
    {

        [Fact]
        public async Task ProcessAsync_NonPostMethod()
        {
            var log = new Mock<ILogger<AnonnymousAuthorizationEndpoint>>();
            var endpoint = new AnonnymousAuthorizationEndpoint(null, null, null, null, log.Object);
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "non-post";

            var res = await endpoint.ProcessAsync(ctx);
            res.ShouldBeOfType<TokenErrorResult>();
        }
        [Fact]
        public async Task ProcessAsync_NoFormContent()
        {
            var log = new Mock<ILogger<AnonnymousAuthorizationEndpoint>>();
            var endpoint = new AnonnymousAuthorizationEndpoint(null, null, null, null, log.Object);
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "post";
            var res = await endpoint.ProcessAsync(ctx);
            res.ShouldBeOfType<TokenErrorResult>();
        }
        [Theory]
        [MemberData(nameof(ProcessAsync_FailedToValidateClient_DATA))]
        public async Task ProcessAsync_FailedToValidateClient(ClientSecretValidationResult csvr)
        {
            var log = new Mock<ILogger<AnonnymousAuthorizationEndpoint>>();
            var cv = new Mock<IClientSecretValidator>();
            cv.Setup(c => c.ValidateAsync(It.IsAny<HttpContext>())).ReturnsAsync(csvr);

            var endpoint = new AnonnymousAuthorizationEndpoint(cv.Object, null, null, null, log.Object);
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
            var log = new Mock<ILogger<AnonnymousAuthorizationEndpoint>>();
            var cv = new Mock<IClientSecretValidator>();
            var csvr = new ClientSecretValidationResult
            {
                Client = new IdentityServer4.Models.Client(),
                IsError = false,
            };
            cv.Setup(c => c.ValidateAsync(It.IsAny<HttpContext>())).ReturnsAsync(csvr);

            var rv = new Mock<IAnonnymousAuthorizationRequestValidator>();
            var arvr = new AuthorizationRequestValidationResult(null, "err");
            rv.Setup(r => r.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClientSecretValidationResult>()))
                .ReturnsAsync(arvr);

            var evt = new Mock<IEventService>();

            var endpoint = new AnonnymousAuthorizationEndpoint(cv.Object, rv.Object, null, evt.Object, log.Object);
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "post";
            ctx.Request.ContentType = "application/x-www-form-urlencoded";
            var fields = new Dictionary<string, StringValues>();
            ctx.Request.Form = new FormCollection(fields);

            var res = await endpoint.ProcessAsync(ctx);
            res.ShouldBeOfType<TokenErrorResult>();
            evt.Verify(e => e.RaiseAsync(It.IsAny<AnonnymousAuthorizationFailureEvent>()), Times.Once);
        }
        [Fact]
        public async Task ProcessAsync_AuthorizeSuccess()
        {
            var log = new Mock<ILogger<AnonnymousAuthorizationEndpoint>>();
            var cv = new Mock<IClientSecretValidator>();
            var csvr = new ClientSecretValidationResult
            {
                Client = new IdentityServer4.Models.Client(),
                IsError = false,
            };
            cv.Setup(c => c.ValidateAsync(It.IsAny<HttpContext>())).ReturnsAsync(csvr);

            var rv = new Mock<IAnonnymousAuthorizationRequestValidator>();
            var arvr = new AuthorizationRequestValidationResult(null);
            rv.Setup(r => r.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClientSecretValidationResult>()))
                .ReturnsAsync(arvr);

            var evt = new Mock<IEventService>();
            var rg = new Mock<IAuthorizationResponseGenerator>();
            rg.Setup(r => r.ProcessAsync(It.IsAny<AuthorizationRequestValidationResult>(), It.IsAny<string>()))
                .ReturnsAsync(new AuthorizationResponse());

            var endpoint = new AnonnymousAuthorizationEndpoint(cv.Object, rv.Object, rg.Object, evt.Object, log.Object);
            var ctx = new DefaultHttpContext();
            ctx.Request.Method = "post";
            ctx.Request.ContentType = "application/x-www-form-urlencoded";
            var fields = new Dictionary<string, StringValues>();
            ctx.Request.Form = new FormCollection(fields);

            var res = await endpoint.ProcessAsync(ctx);

            res.ShouldBeOfType<AuthorizationResult>();
            evt.Verify(e => e.RaiseAsync(It.IsAny<AnonnymousAuthorizationSuccessEvent>()), Times.Once);
            evt.Verify(e => e.RaiseAsync(It.IsAny<AnonnymousAuthorizationFailureEvent>()), Times.Never);
        }
    }
}
