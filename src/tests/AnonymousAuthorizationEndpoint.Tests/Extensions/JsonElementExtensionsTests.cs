using Shouldly;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace Identityserver4.Anonymous.Tests.Extensions
{
    public class JsonElementExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void PropertyStringValueEqualsTo_EmptyPropertyName(string pn)
        {
            var e = new JsonElement();
            Should.Throw<ArgumentException>(() => JsonElementExtensions.PropertyStringValueEqualsTo(e, pn, "value"));
        }

        [Fact]
        public void PropertyStringValueEqualsTo_PropertyNotExists()
        {
            var e = JsonDocument.Parse("{\"key\":\"va;ue\"}").RootElement;
            Should.Throw<KeyNotFoundException>(() => JsonElementExtensions.PropertyStringValueEqualsTo(e, "pn", "value"));
        }
        [Fact]
        public void PropertyStringValueEqualsTo_NotEquals()
        {
            var e = JsonDocument.Parse("{\"k\":\"v\"}").RootElement;
            JsonElementExtensions.PropertyStringValueEqualsTo(e, "k", "value").ShouldBeFalse();
        }

        [Fact]
        public void PropertyStringValueEqualsTo_NotEquals_CaseSensitive()
        {
            var e = JsonDocument.Parse("{\"k\":\"v\"}").RootElement;
            JsonElementExtensions.PropertyStringValueEqualsTo(e, "k", "V", StringComparison.Ordinal).ShouldBeFalse();
        }
        [Fact]
        public void PropertyStringValueEqualsTo_Equals_CaseInsensitive()
        {
            var e = JsonDocument.Parse("{\"k\":\"v\"}").RootElement;
            JsonElementExtensions.PropertyStringValueEqualsTo(e, "k", "V").ShouldBeTrue();
        }
        [Fact]
        public void PropertyStringValueEqualsTo_Equals_CaseSensitive()
        {
            var e = JsonDocument.Parse("{\"k\":\"v\"}").RootElement;
            JsonElementExtensions.PropertyStringValueEqualsTo(e, "k", "v", StringComparison.Ordinal).ShouldBeTrue();
        }
    }
}
