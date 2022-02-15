using IdentityServer4.Models;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace PhoneAuthorizationEndpoint.Tests.Extensions
{
    public class ClientExtensionsTests
    {
        [Theory]
        [InlineData(default)]
        [InlineData("")]
        [InlineData(" ")]
        public void TryGetBooleanPropertyOrDefault_ReturnsDefault_OnMissingProperty(string pn)
        {
            (null as Client).TryGetBooleanPropertyOrDefault(pn).ShouldBeFalse();
        }
        [Fact]
        public void TryGetBooleanPropertyOrDefault_ReturnsDefault_OnNullClient()
        {
            (null as Client).TryGetBooleanPropertyOrDefault("pn").ShouldBeFalse();
        }
        [Fact]
        public void TryGetBooleanPropertyOrDefault_ReturnsDefault_OnNoClientProperties()
        {
            (new Client()).TryGetBooleanPropertyOrDefault("pn").ShouldBeFalse();
        }
        [Fact]
        public void TryGetBooleanPropertyOrDefault_ReturnsDefault_OnMissingClientProperty()
        {
            new Client
            {
                Properties = new Dictionary<string, string> { { "k", "v" } }
            }.TryGetBooleanPropertyOrDefault("pn").ShouldBeFalse();
        }
        [Fact]
        public void TryGetBooleanPropertyOrDefault_ReturnsDefault_OnParseFailure()
        {
            new Client
            {
                Properties = new Dictionary<string, string> { { "k", "v" } }
            }.TryGetBooleanPropertyOrDefault("k").ShouldBeFalse();
        }
        [Theory]
        [InlineData("true")]
        [InlineData("True")]
        public void TryGetBooleanPropertyOrDefault_ReturnsValueSuccessFully(string value)
        {
            new Client
            {
                Properties = new Dictionary<string, string> { { "k", value } }
            }.TryGetBooleanPropertyOrDefault("k").ShouldBeTrue();
        }

        [Theory]
        [InlineData(default)]
        [InlineData("")]
        [InlineData(" ")]
        public void TryGetIntPropertyOrDefault_ReturnsDefault_OnMissingProperty(string pn)
        {
            (null as Client).TryGetIntPropertyOrDefault(pn).ShouldBe(default);
        }
        [Fact]
        public void TryGetIntPropertyOrDefault_ReturnsDefault_OnNullClient()
        {
            (null as Client).TryGetIntPropertyOrDefault("pn").ShouldBe(default);
        }
        [Fact]
        public void TryGetIntPropertyOrDefault_ReturnsDefault_OnNoClientProperties()
        {
            new Client().TryGetIntPropertyOrDefault("pn").ShouldBe(default);
        }
        [Fact]
        public void TryGetIntPropertyOrDefault_ReturnsDefault_OnMissingClientProperty()
        {
            new Client
            {
                Properties = new Dictionary<string, string> { { "k", "v" } }
            }.TryGetIntPropertyOrDefault("pn").ShouldBe(default);
        }
        [Fact]
        public void TryGetIntPropertyOrDefault_ReturnsDefault_OnParseFailure()
        {
            new Client
            {
                Properties = new Dictionary<string, string> { { "k", "v" } }
            }.TryGetIntPropertyOrDefault("k").ShouldBe(default);
        }
        [Fact]
        public void TryGetIntPropertyOrDefault_ReturnsValueSuccessFully()
        {
            new Client
            {
                Properties = new Dictionary<string, string> { { "k", "1" } }
            }.TryGetIntPropertyOrDefault("k").ShouldBe(1);
        }

        [Theory]
        [InlineData(default)]
        [InlineData("")]
        [InlineData(" ")]
        public void TryGetStringPropertyOrDefault_ReturnsDefault_OnMissingProperty(string pn)
        {
            (null as Client).TryGetStringPropertyOrDefault(pn).ShouldBe(default);
        }
        [Fact]
        public void TryGetStringPropertyOrDefault_ReturnsDefault_OnNullClient()
        {
            (null as Client).TryGetStringPropertyOrDefault("pn").ShouldBe(default);
        }
        [Fact]
        public void TryGetStringPropertyOrDefault_ReturnsDefault_OnNoClientProperties()
        {
            new Client().TryGetStringPropertyOrDefault("pn").ShouldBe(default);
        }
        [Fact]
        public void TryGetStringPropertyOrDefault_ReturnsDefault_OnMissingClientProperty()
        {
            new Client
            {
                Properties = new Dictionary<string, string> { { "k", "v" } }
            }.TryGetStringPropertyOrDefault("pn").ShouldBe(default);
        }
        [Fact]
        public void TryGetStringPropertyOrDefault_ReturnsValueSuccessFully()
        {
            var value = "v";
            new Client
            {
                Properties = new Dictionary<string, string> { { "k", value } }
            }.TryGetStringPropertyOrDefault("k").ShouldBe(value);
        }
    }
}
