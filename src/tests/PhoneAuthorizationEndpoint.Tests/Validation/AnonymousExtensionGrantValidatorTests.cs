using IdentityServer4.Anonymous.Services;
using IdentityServer4.Anonymous.Stores;
using IdentityServer4.Anonymous.Validation;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.Anonymous.Tests.Validation
{
    public class AnonymousExtensionGrantValidatorTests
    {
        [Fact]
        public void GrantTypeValue()
        {
            new AnonymousExtensionGrantValidator(default, default, default, default).GrantType.ShouldBe("anonymous");
        }
        [Theory]
        [MemberData(nameof(ValidateAsync_Error_OnNulls_DATA))]
        public async Task ValidateAsync_Error_OnNulls(ExtensionGrantValidationContext context)
        {
            var log = new Mock<ILogger<AnonymousExtensionGrantValidator>>();
            var v = new AnonymousExtensionGrantValidator(default, default, default, log.Object);
            await Should.ThrowAsync<NullReferenceException>(() => v.ValidateAsync(context));
        }

        public static IEnumerable<object[]> ValidateAsync_Error_OnNulls_DATA = new[]
        {
            new object[]{null },
            new object[]{new ExtensionGrantValidationContext{Request = default} },
            new object[]{new ExtensionGrantValidationContext{Request = new ValidatedTokenRequest()} },
        };
        [Theory]
        [MemberData(nameof(ValidateAsync_Error_OnMissingToken_DATA))]
        public async Task ValidateAsync_Error_OnMissingToken(ExtensionGrantValidationContext context)
        {
            var log = new Mock<ILogger<AnonymousExtensionGrantValidator>>();
            var v = new AnonymousExtensionGrantValidator(default, default, default, log.Object);
            await v.ValidateAsync(context);
            context.Result.IsError.ShouldBeTrue();
        }
        public static IEnumerable<object[]> ValidateAsync_Error_OnMissingToken_DATA = new[]
       {
            new object[]{new ExtensionGrantValidationContext
                {
                    Request = new ValidatedTokenRequest
                    {
                        Raw = new NameValueCollection { ["invalid"] = "test" }
                    }
                }
            },
            new object[]{new ExtensionGrantValidationContext
                {
                    Request = new ValidatedTokenRequest
                    {
                        Raw = new NameValueCollection { ["token"] = default }
                    }
                }
            },new object[]{new ExtensionGrantValidationContext
                {
                    Request = new ValidatedTokenRequest
                    {
                        Raw = new NameValueCollection { ["token"] = "" }
                    }
                }
            },new object[]{new ExtensionGrantValidationContext
                {
                    Request = new ValidatedTokenRequest
                    {
                        Raw = new NameValueCollection { ["token"] = " " }
                    }
                }
            },
        };

        [Fact]
        public async Task ValidateAsync_Error_OnInvalid()
        {
            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new NameValueCollection { ["token"] = "value" }
                }
            };
            var log = new Mock<ILogger<AnonymousExtensionGrantValidator>>();
            var tv = new Mock<ITokenValidator>();
            tv.Setup(t => t.ValidateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new TokenValidationResult { IsError = true });

            var v = new AnonymousExtensionGrantValidator(tv.Object, default, default, log.Object);
            await v.ValidateAsync(context);
            context.Result.IsError.ShouldBeTrue();
        }
        [Theory]
        [MemberData(nameof(ValidateAsync_Error_OnInvalidUserCode_DATA))]
        public async Task ValidateAsync_Error_OnInvalidUserCode(ExtensionGrantValidationContext context)
        {
            var log = new Mock<ILogger<AnonymousExtensionGrantValidator>>();
            var tv = new Mock<ITokenValidator>();
            tv.Setup(t => t.ValidateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new TokenValidationResult { IsError = false });

            var v = new AnonymousExtensionGrantValidator(tv.Object, default, default, log.Object);
            await v.ValidateAsync(context);
            context.Result.IsError.ShouldBeTrue();
        }
        public static IEnumerable<object[]> ValidateAsync_Error_OnInvalidUserCode_DATA = new[]
       {
            new object[]{new ExtensionGrantValidationContext
                {
                    Request = new ValidatedTokenRequest
                    {
                        Raw = new NameValueCollection
                        {
                            ["token"] = "t",
                        }
                    }
                }
            },
            new object[]{new ExtensionGrantValidationContext
                {
                    Request = new ValidatedTokenRequest
                    {
                        Raw = new NameValueCollection {
                            ["token"] = "t",
                            ["user_code"] = default
                        }
                    }
                }
            },new object[]{new ExtensionGrantValidationContext
                {
                    Request = new ValidatedTokenRequest
                    {
                        Raw = new NameValueCollection {
                            ["token"] = "t",
                            ["user_code"] = "" }
                    }
                }
            },new object[]{new ExtensionGrantValidationContext
                {
                    Request = new ValidatedTokenRequest
                    {
                        Raw = new NameValueCollection {
                            ["token"] = "t",
                            ["user_code"] = " "
                        }
                    }
                }
            },
        };
        [Theory]
        [MemberData(nameof(ValidateAsync_Error_OnInvalidClaims_DATA))]
        public async Task ValidateAsync_Error_OnInvalidClaims(IEnumerable<Claim> claims)
        {
            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new NameValueCollection
                    {
                        ["token"] = "t",
                        ["user_code"] = "uc"
                    }
                }
            };
            var log = new Mock<ILogger<AnonymousExtensionGrantValidator>>();
            var tv = new Mock<ITokenValidator>();
            tv.Setup(t => t.ValidateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new TokenValidationResult { IsError = false, Claims = claims });

            var v = new AnonymousExtensionGrantValidator(tv.Object, default, default, log.Object);
            await v.ValidateAsync(context);
            context.Result.IsError.ShouldBeTrue();
        }
        public static IEnumerable<object[]> ValidateAsync_Error_OnInvalidClaims_DATA = new[]
        {
            new object[] {null },
            new object[] { Array.Empty<Claim>()},
            new object[] { new[] { new Claim("t", "g") } },
        };

        [Fact]
        public async Task ValidateAsync_Error_OnFailureFetchVerificationCode()
        {
            var context = new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Raw = new NameValueCollection
                    {
                        ["token"] = "t",
                        ["user_code"] = "uc"
                    }
                }
            };
            var log = new Mock<ILogger<AnonymousExtensionGrantValidator>>();
            var tvRes = new TokenValidationResult { IsError = false, Claims = new[] { new Claim("amr", "phone") } };
            var tv = new Mock<ITokenValidator>();
            tv.Setup(t => t.ValidateAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(tvRes);

            var cs = new Mock<IAnonymousCodeStore>();
            cs.Setup(c => c.FindByVerificationCodeAsync(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(default(AnonymousCodeInfo));
            var v = new AnonymousExtensionGrantValidator(tv.Object, cs.Object, default, log.Object);
            await v.ValidateAsync(context);
            context.Result.IsError.ShouldBeTrue();
        }
    }
}