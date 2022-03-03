using Shouldly;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xunit;

namespace IdentityServer4.Anonymous.Tests.Extensions
{
    public class NameValueCollectionExtensionsTests
    {
        [Theory]
        [MemberData(nameof(ToScrubbedDictionary_ReturnsEmpty_OnNullOrEmptyCollection_DATA))]
        public void ToScrubbedDictionary_ReturnsEmpty_OnNullOrEmptyCollection(NameValueCollection c)
        {
            NameValueCollectionExtensions.ToScrubbedDictionary(c).ShouldBeEmpty();
        }

        public static IEnumerable<object[]> ToScrubbedDictionary_ReturnsEmpty_OnNullOrEmptyCollection_DATA = new[]
        {
            new object[] { null },
            new object[] {new NameValueCollection()}
        };
        [Fact]
        public void ToScrubbedDictionary_ReturnsEmpty_NoFilters()
        {
            var c = new NameValueCollection
            {
                ["k1"] = "v1",
                ["k2"] = "v2",
            };
            var d = NameValueCollectionExtensions.ToScrubbedDictionary(c);
            d.Count.ShouldBe(c.Count);
            d["k1"].ShouldBe("v1");
            d["k2"].ShouldBe("v2");
        }
        [Fact]
        public void ToScrubbedDictionary_ReturnsEmpty_OnFilters()
        {
            var c = new NameValueCollection
            {
                ["k1"] = "v1",
                ["k2"] = "v2",
            };
            var d = NameValueCollectionExtensions.ToScrubbedDictionary(c, "k2");
            d.Count.ShouldBe(c.Count);
            d["k1"].ShouldBe("v1");
            d["k2"].ShouldBe("***REDACTED***");
        }
    }
}
