using IdentityServer4;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace PhoneAuthorizationEndpoint.Tests.Extensions
{
    public class ClaimExtensionsTests
    {
        [Theory]
        [MemberData(nameof(GetFirstOrDefault_ReturnsDefault_OnEmptyOrNullcollection_DATA))]
        public void GetFirstOrDefault_ReturnsDefault_OnEmptyOrNullcollection(IEnumerable<Claim> claims)
        {
            ClaimExtensions.GetFirstOrDefault(claims, "t", default).ShouldBe(default);
        }
        public static IEnumerable<object[]> GetFirstOrDefault_ReturnsDefault_OnEmptyOrNullcollection_DATA => new[]
        {
            new object[]{null},
            new object[]{Array.Empty<Claim>()},
        };

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetFirstOrDefault_ReturnsDefault_MissingClaimType(string type)
        {
            var claims = new[]
            {
                new Claim("k1", "v1"),
                new Claim("k2", "v2"),
                new Claim("k3", "v3"),
            };
            ClaimExtensions.GetFirstOrDefault(claims, type, default).ShouldBe(default);
        }
        [Fact]
        public void GetFirstOrDefault_ReturnsDefault_OnMissingClaimType()
        {
            var claims = new[]
            {
                new Claim("k1", "v1"),
                new Claim("k2", "v2"),
                new Claim("k3", "v3"),
            };
            ClaimExtensions.GetFirstOrDefault(claims, "t", default).ShouldBe(default);
        }
        [Fact]
        public void GetFirstOrDefault_ReturnsFirstValue()
        {
            string k = "key",
                v = "v1";

            var claims = new[]
            {
                new Claim(k, v),
                new Claim(k, "v2"),
                new Claim(k, "v3"),
            };
            var c = ClaimExtensions.GetFirstOrDefault(claims, k, default);
            c.Type.ShouldBe(k);
            c.Value.ShouldBe(v);
        }

        [Theory]
        [MemberData(nameof(GetFirstValueOrDefault_ReturnsDefault_OnEmptyOrNullcollection_DATA))]
        public void GetFirstValueOrDefault_ReturnsDefault_OnEmptyOrNullcollection(IEnumerable<Claim> claims)
        {
            ClaimExtensions.GetFirstValueOrDefault(claims, "t", default).ShouldBe(default);
        }
        public static IEnumerable<object[]> GetFirstValueOrDefault_ReturnsDefault_OnEmptyOrNullcollection_DATA => new[]
        {
            new object[]{null},
            new object[]{Array.Empty<Claim>()},
        };

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetFirstValueOrDefault_ReturnsDefault_MissingClaimType(string type)
        {
            var claims = new[]
            {
                new Claim("k1", "v1"),
                new Claim("k2", "v2"),
                new Claim("k3", "v3"),
            };
            ClaimExtensions.GetFirstValueOrDefault(claims, type, default).ShouldBe(default);
        }
        [Fact]
        public void GetFirstValueOrDefault_ReturnsDefault_OnMissingClaimType()
        {
            var claims = new[]
            {
                new Claim("k1", "v1"),
                new Claim("k2", "v2"),
                new Claim("k3", "v3"),
            };
            ClaimExtensions.GetFirstValueOrDefault(claims, "t", default).ShouldBe(default);
        }
        [Fact]
        public void GetFirstValueOrDefault_ReturnsFirstValue()
        {
            string k = "key",
                v = "v1";

            var claims = new[]
            {
                new Claim(k, v),
                new Claim(k, "v2"),
                new Claim(k, "v3"),
            };
            var c = ClaimExtensions.GetFirstValueOrDefault(claims, k, default);
            c.ShouldBe(v);
        }
    }
}
