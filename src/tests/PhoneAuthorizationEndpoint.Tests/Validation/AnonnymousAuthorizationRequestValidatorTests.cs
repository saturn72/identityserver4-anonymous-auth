using IdentityServer4.Anonnymous.Validation;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.Anonnymous.Tests.Validation
{
    public class AnonnymousAuthorizationRequestValidatorTests
    {
        [Fact]
        public async Task ValidateAsync_ThrowsOnMissingParameters()
        {
            var log = new Mock<ILogger<AnonnymousAuthorizationRequestValidator>>();
            var oData = new AnonnymousAuthorizationOptions
            {
                Transports = new[] { "t1", "t2" }
            };
            var opt = new Mock<IOptions<AnonnymousAuthorizationOptions>>();
            opt.Setup(o => o.Value).Returns(oData);

            var v = new AnonnymousAuthorizationRequestValidator(opt.Object, null, log.Object);

            await Should.ThrowAsync<ArgumentNullException>(() => v.ValidateAsync(default, default));
        }
        [Fact]
        public async Task ValidateAsync_ReturnErrorOnMissingTransportNameValueEntry()
        {
            var log = new Mock<ILogger<AnonnymousAuthorizationRequestValidator>>();
            var oData = new AnonnymousAuthorizationOptions
            {
                Transports = new[] { "t1", "t2" }
            };
            var opt = new Mock<IOptions<AnonnymousAuthorizationOptions>>();
            opt.Setup(o => o.Value).Returns(oData);

            var v = new AnonnymousAuthorizationRequestValidator(opt.Object, null, log.Object);

            var r = await v.ValidateAsync(new NameValueCollection(), default);
            r.IsError.ShouldBeTrue();
        }
        [Fact]
        public async Task ValidateAsync_ReturnErrorOnNotSupporttedTransportNameValueEntry()
        {
            var log = new Mock<ILogger<AnonnymousAuthorizationRequestValidator>>();
            var oData = new AnonnymousAuthorizationOptions
            {
                Transports = new[] { "t1", "t2" }
            };
            var opt = new Mock<IOptions<AnonnymousAuthorizationOptions>>();
            opt.Setup(o => o.Value).Returns(oData);

            var v = new AnonnymousAuthorizationRequestValidator(opt.Object, null, log.Object);

            var nvc = new NameValueCollection
            {
                [Constants.FormParameters.Transport] = "ttt"
            };
            var r = await v.ValidateAsync(nvc, default);
            r.IsError.ShouldBeTrue();
        }
        [Fact]
        public async Task ValidateAsync_ReturnErrorOnMissingProvider()
        {
            var log = new Mock<ILogger<AnonnymousAuthorizationRequestValidator>>();
            var oData = new AnonnymousAuthorizationOptions
            {
                Transports = new[] { "t1", "t2" }
            };
            var opt = new Mock<IOptions<AnonnymousAuthorizationOptions>>();
            opt.Setup(o => o.Value).Returns(oData);

            var v = new AnonnymousAuthorizationRequestValidator(opt.Object, null, log.Object);

            var nvc = new NameValueCollection
            {
                [Constants.FormParameters.Transport] = "t1"
            };
            var r = await v.ValidateAsync(nvc, default);
            r.IsError.ShouldBeTrue();
        }
        [Theory]
        [MemberData(nameof(ValidateAsync_ThrowsOnInvalidClient_DATA))]
        public async Task ValidateAsync_ThrowsOnInvalidClient(ClientSecretValidationResult csvr)
        {
            var log = new Mock<ILogger<AnonnymousAuthorizationRequestValidator>>();
            var oData = new AnonnymousAuthorizationOptions
            {
                Transports = new[] { "t1", "t2" }
            };
            var opt = new Mock<IOptions<AnonnymousAuthorizationOptions>>();
            opt.Setup(o => o.Value).Returns(oData);

            var v = new AnonnymousAuthorizationRequestValidator(opt.Object, null, log.Object);

            var nvc = new NameValueCollection
            {
                [Constants.FormParameters.Transport] = "t1",
                [Constants.FormParameters.Provider] = "p"
            };
            await Should.ThrowAsync<ArgumentNullException>(() => v.ValidateAsync(nvc, csvr));
        }
        public static IEnumerable<object[]> ValidateAsync_ThrowsOnInvalidClient_DATA = new[]
        {
            new object[] { default, },
            new object[] { new ClientSecretValidationResult() },
        };
        [Fact]
        public async Task ValidateAsync_ReturnsError_ClientIs_NoOpenIdProtocol()
        {
            var csvr = new ClientSecretValidationResult
            {
                Client = new Client(),
            };
            var log = new Mock<ILogger<AnonnymousAuthorizationRequestValidator>>();
            var oData = new AnonnymousAuthorizationOptions
            {
                Transports = new[] { "t1", "t2" }
            };
            var opt = new Mock<IOptions<AnonnymousAuthorizationOptions>>();
            opt.Setup(o => o.Value).Returns(oData);

            var v = new AnonnymousAuthorizationRequestValidator(opt.Object, null, log.Object);

            var nvc = new NameValueCollection
            {
                [Constants.FormParameters.Transport] = "t1",
                [Constants.FormParameters.Provider] = "p"
            };
            var res = await v.ValidateAsync(nvc, csvr);
            res.IsError.ShouldBeTrue();
        }
        [Fact]
        public async Task ValidateAsync_ReturnsError_MissingAnonnymousAllowedGrant_ForClient()
        {
            var csvr = new ClientSecretValidationResult
            {
                Client = new Client
                {
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect
                },
            };
            var log = new Mock<ILogger<AnonnymousAuthorizationRequestValidator>>();
            var oData = new AnonnymousAuthorizationOptions
            {
                Transports = new[] { "t1", "t2" }
            };
            var opt = new Mock<IOptions<AnonnymousAuthorizationOptions>>();
            opt.Setup(o => o.Value).Returns(oData);

            var v = new AnonnymousAuthorizationRequestValidator(opt.Object, null, log.Object);

            var nvc = new NameValueCollection
            {
                [Constants.FormParameters.Transport] = "t1",
                [Constants.FormParameters.Provider] = "p"
            };
            var res = await v.ValidateAsync(nvc, csvr);
            res.IsError.ShouldBeTrue();
        }
        [Fact]
        public async Task ValidateAsync_ReturnsError_MissingTransports_ForClient()
        {
            var csvr = new ClientSecretValidationResult
            {
                Client = new Client
                {
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    AllowedGrantTypes = { "anonnymous" },
                },
            };
            var log = new Mock<ILogger<AnonnymousAuthorizationRequestValidator>>();
            var oData = new AnonnymousAuthorizationOptions
            {
                Transports = new[] { "t1", "t2" }
            };
            var opt = new Mock<IOptions<AnonnymousAuthorizationOptions>>();
            opt.Setup(o => o.Value).Returns(oData);

            var v = new AnonnymousAuthorizationRequestValidator(opt.Object, null, log.Object);

            var nvc = new NameValueCollection
            {
                [Constants.FormParameters.Transport] = "t1",
                [Constants.FormParameters.Provider] = "p"
            };
            var res = await v.ValidateAsync(nvc, csvr);
            res.IsError.ShouldBeTrue();
        }
        [Theory]
        [InlineData(default)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(" invalid json")]
        public async Task ValidateAsync_ReturnsError_BlankTransports_ForClient(string value)
        {
            var csvr = new ClientSecretValidationResult
            {
                Client = new Client
                {
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    AllowedGrantTypes = { "anonnymous" },
                    Properties = new Dictionary<string, string> { { "transport", value } },
                },
            };
            var log = new Mock<ILogger<AnonnymousAuthorizationRequestValidator>>();
            var oData = new AnonnymousAuthorizationOptions
            {
                Transports = new[] { "t1", "t2" }
            };
            var opt = new Mock<IOptions<AnonnymousAuthorizationOptions>>();
            opt.Setup(o => o.Value).Returns(oData);

            var v = new AnonnymousAuthorizationRequestValidator(opt.Object, null, log.Object);

            var nvc = new NameValueCollection
            {
                [Constants.FormParameters.Transport] = "t1",
                [Constants.FormParameters.Provider] = "p"
            };
            var res = await v.ValidateAsync(nvc, csvr);
            res.IsError.ShouldBeTrue();
        }
        /* 
        //validate client
        var clientResult = ValidateClient(request, clientValidationResult);
        if (clientResult.IsError)
            return clientResult;

        //validate scope
        var scopeResult = await ValidateScopeAsync(request);
        if (scopeResult.IsError)
        {
            return scopeResult;
        }
        _logger.LogDebug("{clientId} anonnymous request validation success", request.Client.ClientId);
        return Valid(request);
    }*/
    }
}
