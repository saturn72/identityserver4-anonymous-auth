using IdentityModel;
using IdentityServer4.Anonnymous.Validation;
using IdentityServer4.Configuration;
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
        [InlineData(" invalid transport json")]
        public async Task ValidateAsync_ReturnsError_BlankTransports_ForClient(string value)
        {
            var csvr = new ClientSecretValidationResult
            {
                Client = new Client
                {
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    AllowedGrantTypes = { "anonnymous" },
                    Properties = new Dictionary<string, string> { { "transports", value } },
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
        [InlineData(" no-match-provider")]
        public async Task ValidateAsync_ReturnsError_NoMatchingProvider(string value)
        {
            var t = string.Format("[{{\"name\":\"t1\", \"provider\":\"{0}\", \"config\":{{\"key\":\"twilio\"}}}}]", value);
            var csvr = new ClientSecretValidationResult
            {
                Client = new Client
                {
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    AllowedGrantTypes = { "anonnymous" },
                    Properties = new Dictionary<string, string> {
                        { "transports", t },
                    },
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
                [Constants.FormParameters.Provider] = "p1"
            };
            var res = await v.ValidateAsync(nvc, csvr);
            res.IsError.ShouldBeTrue();
        }

        [Theory]
        [InlineData(default)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(" this is not uri")]
        [InlineData("http://not-exists.com/callback")]
        public async Task ValidateAsync_ReturnsError_NoRedirectUri(string value)
        {
            var csvr = new ClientSecretValidationResult
            {
                Client = new Client
                {
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    AllowedGrantTypes = { "anonnymous" },
                    Properties = new Dictionary<string, string> {
                        { "transports", "[{\"name\":\"t1\", \"provider\":\"p\", \"config\":{\"key\":\"twilio\"}}]" },
                    },
                    RedirectUris = { "http://exists.com/callback" }
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
                [Constants.FormParameters.Provider] = "p",
                [Constants.FormParameters.RedirectUri] = value
            };
            var res = await v.ValidateAsync(nvc, csvr);
            res.IsError.ShouldBeTrue();
        }

        [Fact]
        public async Task ValidateAsync_ReturnsError_InvalidScope_NoScopes()
        {
            var csvr = new ClientSecretValidationResult
            {
                Client = new Client
                {
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    AllowedGrantTypes = { "anonnymous" },
                    Properties = new Dictionary<string, string> {
                        { "transports", "[{\"name\":\"t1\", \"provider\":\"p\", \"config\":{\"key\":\"twilio\"}}]" },
                    },
                    RedirectUris = { "http://exists.com/callback" }
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
                [Constants.FormParameters.Provider] = "p",
                [Constants.FormParameters.RedirectUri] = "http://exists.com/callback"
            };
            var res = await v.ValidateAsync(nvc, csvr);
            res.IsError.ShouldBeTrue();
        }

        [Theory]
        [InlineData(default)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ValidateAsync_ReturnsError_InvalidScope_MissingValueScopes(string value)
        {
            var csvr = new ClientSecretValidationResult
            {
                Client = new Client
                {
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    AllowedGrantTypes = { "anonnymous" },
                    Properties = new Dictionary<string, string> {
                        { "transports", "[{\"name\":\"t1\", \"provider\":\"p\", \"config\":{\"key\":\"twilio\"}}]" },
                    },
                    RedirectUris = { "http://exists.com/callback" }
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
                [Constants.FormParameters.Provider] = "p",
                [Constants.FormParameters.RedirectUri] = "http://exists.com/callback",
                [OidcConstants.AuthorizeRequest.Scope] = value
            };
            var res = await v.ValidateAsync(nvc, csvr);
            res.IsError.ShouldBeTrue();
        }
        [Fact]
        public async Task ValidateAsync_ReturnsError_InvalidScope_ScopeToLong()
        {
            var csvr = new ClientSecretValidationResult
            {
                Client = new Client
                {
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    AllowedGrantTypes = { "anonnymous" },
                    Properties = new Dictionary<string, string> {
                        { "transports", "[{\"name\":\"t1\", \"provider\":\"p\", \"config\":{\"key\":\"twilio\"}}]" },
                    },
                    RedirectUris = { "http://exists.com/callback" }
                },
            };
            var log = new Mock<ILogger<AnonnymousAuthorizationRequestValidator>>();
            var oData = new AnonnymousAuthorizationOptions
            {
                Transports = new[] { "t1", "t2" },
                InputLengthRestrictions = new InputLengthRestrictions
                {
                    Scope = 1,
                }
            };
            var opt = new Mock<IOptions<AnonnymousAuthorizationOptions>>();
            opt.Setup(o => o.Value).Returns(oData);

            var v = new AnonnymousAuthorizationRequestValidator(opt.Object, null, log.Object);

            var nvc = new NameValueCollection
            {
                [Constants.FormParameters.Transport] = "t1",
                [Constants.FormParameters.Provider] = "p",
                [Constants.FormParameters.RedirectUri] = "http://exists.com/callback",
                [OidcConstants.AuthorizeRequest.Scope] = "to-long",
            };
            var res = await v.ValidateAsync(nvc, csvr);
            res.IsError.ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(ValidateAsync_ReturnsError_InvalidScope_ResourceValidatorFailure_DATA))]
        public async Task ValidateAsync_ReturnsError_InvalidScope_ResourceValidatorFailure(ResourceValidationResult result)
        {
            var csvr = new ClientSecretValidationResult
            {
                Client = new Client
                {
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    AllowedGrantTypes = { "anonnymous" },
                    Properties = new Dictionary<string, string> {
                        { "transports", "[{\"name\":\"t1\", \"provider\":\"p\", \"config\":{\"key\":\"twilio\"}}]" },
                    },
                    RedirectUris = { "http://exists.com/callback" },
                    AllowedScopes = { "scope-1" },
                },
            };
            var log = new Mock<ILogger<AnonnymousAuthorizationRequestValidator>>();
            var oData = new AnonnymousAuthorizationOptions
            {
                Transports = new[] { "t1", "t2" },
                InputLengthRestrictions = new InputLengthRestrictions
                {
                    Scope = 100,
                }
            };
            var opt = new Mock<IOptions<AnonnymousAuthorizationOptions>>();
            opt.Setup(o => o.Value).Returns(oData);

            var rv = new Mock<IResourceValidator>();
            rv.Setup(r => r.ValidateRequestedResourcesAsync(It.IsAny<ResourceValidationRequest>()))
                .ReturnsAsync(result);

            var v = new AnonnymousAuthorizationRequestValidator(opt.Object, rv.Object, log.Object);

            var nvc = new NameValueCollection
            {
                [Constants.FormParameters.Transport] = "t1",
                [Constants.FormParameters.Provider] = "p",
                [Constants.FormParameters.RedirectUri] = "http://exists.com/callback",
                [OidcConstants.AuthorizeRequest.Scope] = "to-long",
            };
            var res = await v.ValidateAsync(nvc, csvr);
            res.IsError.ShouldBeTrue();
        }
        public static IEnumerable<object[]> ValidateAsync_ReturnsError_InvalidScope_ResourceValidatorFailure_DATA = new[]
        {
            new object[] { null },
            new object[] { new ResourceValidationResult() },
        };

        [Fact]
        public async Task ValidateAsync_ReturnsError_InvalidScope_OnNonOpenIdScope()
        {
            var csvr = new ClientSecretValidationResult
            {
                Client = new Client
                {
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    AllowedGrantTypes = { "anonnymous" },
                    Properties = new Dictionary<string, string> {
                        { "transports", "[{\"name\":\"t1\", \"provider\":\"p\", \"config\":{\"key\":\"twilio\"}}]" },
                    },
                    RedirectUris = { "http://exists.com/callback" },
                    AllowedScopes = { "scope-1" },
                },
            };
            var log = new Mock<ILogger<AnonnymousAuthorizationRequestValidator>>();
            var oData = new AnonnymousAuthorizationOptions
            {
                Transports = new[] { "t1", "t2" },
                InputLengthRestrictions = new InputLengthRestrictions
                {
                    Scope = 100,
                }
            };
            var opt = new Mock<IOptions<AnonnymousAuthorizationOptions>>();
            opt.Setup(o => o.Value).Returns(oData);

            var vrResult = new ResourceValidationResult();
            vrResult.ParsedScopes.Add(new ParsedScopeValue("x"));
            vrResult.Resources.IdentityResources.Add(new IdentityResource());

            var rv = new Mock<IResourceValidator>();
            rv.Setup(r => r.ValidateRequestedResourcesAsync(It.IsAny<ResourceValidationRequest>()))
                .ReturnsAsync(vrResult);

            var v = new AnonnymousAuthorizationRequestValidator(opt.Object, rv.Object, log.Object);

            var nvc = new NameValueCollection
            {
                [Constants.FormParameters.Transport] = "t1",
                [Constants.FormParameters.Provider] = "p",
                [Constants.FormParameters.RedirectUri] = "http://exists.com/callback",
                [OidcConstants.AuthorizeRequest.Scope] = "to-long",
            };
            var res = await v.ValidateAsync(nvc, csvr);
            res.IsError.ShouldBeTrue();
        }

        [Fact]
        public async Task ValidateAsync_ValidaRequest()
        {
            var csvr = new ClientSecretValidationResult
            {
                Client = new Client
                {
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    AllowedGrantTypes = { "anonnymous" },
                    Properties = new Dictionary<string, string> {
                        { "transports", "[{\"name\":\"t1\", \"provider\":\"p\", \"config\":{\"key\":\"twilio\"}}]" },
                    },
                    RedirectUris = { "http://exists.com/callback" },
                    AllowedScopes = { "scope-1" },
                },
            };
            var log = new Mock<ILogger<AnonnymousAuthorizationRequestValidator>>();
            var oData = new AnonnymousAuthorizationOptions
            {
                Transports = new[] { "t1", "t2" },
                InputLengthRestrictions = new InputLengthRestrictions
                {
                    Scope = 100,
                }
            };
            var opt = new Mock<IOptions<AnonnymousAuthorizationOptions>>();
            opt.Setup(o => o.Value).Returns(oData);

            var vrResult = new ResourceValidationResult();
            vrResult.ParsedScopes.Add(new ParsedScopeValue("x"));

            var rv = new Mock<IResourceValidator>();
            rv.Setup(r => r.ValidateRequestedResourcesAsync(It.IsAny<ResourceValidationRequest>()))
                .ReturnsAsync(vrResult);

            var v = new AnonnymousAuthorizationRequestValidator(opt.Object, rv.Object, log.Object);

            var nvc = new NameValueCollection
            {
                [Constants.FormParameters.Transport] = "t1",
                [Constants.FormParameters.Provider] = "p",
                [Constants.FormParameters.RedirectUri] = "http://exists.com/callback",
                [OidcConstants.AuthorizeRequest.Scope] = "to-long",
            };
            var res = await v.ValidateAsync(nvc, csvr);
            res.IsError.ShouldBeFalse();
        }
    }
}
