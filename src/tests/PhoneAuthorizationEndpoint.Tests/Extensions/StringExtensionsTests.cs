using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace PhoneAuthorizationEndpoint.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void StringExtensions_HasValue_ReturnsFalse(string source)
        {
            source.HasValue().ShouldBeFalse();
        }

        [Fact]
        public void StringExtensions_HasValue_ReturnsTrue()
        {
            "test_string".HasValue().ShouldBeTrue();
        }
        [Fact]
        public void TryParseAsJsonDocument_ReturnsFalse()
        {
            var j = "";
            StringExtensions.TryParseAsJsonDocument(j, out var value).ShouldBeFalse();
            value.ShouldBe(default);
        }
        [Fact]
        public void TryParseAsJsonDocument_ReturnsTrue()
        {
            var j = "\"k\":\"v\"";
            StringExtensions.TryParseAsJsonDocument(j, out var value).ShouldBeTrue();
            value.ToString().ShouldBe(j);
        }
        [Fact]
        public void EnsureLeadingSlash_ThrowsOnNull()
        {
            Should.Throw<ArgumentNullException>(() => StringExtensions.EnsureLeadingSlash(default));
        }
        [Fact]
        public void EnsureLeadingSlash_ThrowsOnTooShort()
        {
            Should.Throw<IndexOutOfRangeException>(() => StringExtensions.EnsureLeadingSlash(default));
        }
        [Fact]
        public void EnsureLeadingSlash_AddsLeadingSlash()
        {
            StringExtensions.EnsureLeadingSlash("s").ShouldBe("/s");
        }
        [Fact]
        public void EnsureLeadingSlash_DoNothingOnExistsLeadingSlash()
        {
            StringExtensions.EnsureLeadingSlash("/s").ShouldBe("/s");
        }
        [Fact]
        public void EnsureTrailingSlash_ThrowsOnNull()
        {
            Should.Throw<ArgumentNullException>(() => StringExtensions.EnsureTrailingSlash(default));
        }
        [Fact]
        public void EnsureTrailingSlash_ThrowsOnTooShort()
        {
            Should.Throw<IndexOutOfRangeException>(() => StringExtensions.EnsureTrailingSlash(default));
        }
        [Fact]
        public void EnsureTrailingSlash_AddOnMissingTrailingSlash()
        {
            StringExtensions.EnsureTrailingSlash("s").ShouldBe("s/");
        }

        [Fact]
        public void EnsureTrailingSlash_DoNothingOnTrailingSlashExists()
        {
            StringExtensions.EnsureTrailingSlash("s/").ShouldBe("s/");
        }

        [Fact]
        public void RemoveTrailingSlash_ThrowsOnNull()
        {
            Should.Throw<ArgumentNullException>(() => StringExtensions.RemoveTrailingSlash(default));
        }
        [Fact]
        public void RemoveTrailingSlash_ThrowsOnTooShort()
        {
            Should.Throw<IndexOutOfRangeException>(() => StringExtensions.RemoveTrailingSlash(default));
        }
        [Fact]
        public void RemoveTrailingSlash_RemovesOnTrailingSlashExists()
        {
            StringExtensions.RemoveTrailingSlash("s/").ShouldBe("s");
        }

        [Fact]
        public void RemoveTrailingSlash_DoNothingOnTrailingSlashNotExists()
        {
            StringExtensions.RemoveTrailingSlash("s").ShouldBe("s");
        }
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("invalid")]
        public void TryConvertToUri_ReturnsFalse_OnInvalidUri(string invalidUri)
        {
            StringExtensions.TryConvertToUri(invalidUri, out var value).ShouldBeFalse();
            value.ShouldBe(default);
        }
        [Fact]
        public void TryConvertToUri_ReturnsTrue()
        {
            var u = "https://saturn72.com";
            StringExtensions.TryConvertToUri(u, out var value).ShouldBeTrue();
            value.ToString().ShouldBe(u);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void FromDelimitedString_ReturnsSameOnEmptyString(string str)
        {
            str.FromDelimitedString("dd").ShouldBe(Array.Empty<string>());
        }
        [Fact]
        public void FromDelimitedString_ReturnsNullOnNullSource()
        {
            (null as string).FromDelimitedString("d").ShouldBeNull();
        }
        [Theory]
        [InlineData("a", "a", "ssssss")]
        [InlineData("abcd", "abcd", "")]
        public void FromDelimitedString_SingleItemArray(string input, string output, string d)
        {
            var o = input.FromDelimitedString(d);
            o.Count().ShouldBe(1);
            o.First().ShouldBe(output);
        }
        [Fact]
        public void FromDelimitedString_NotNullDelimiter()
        {
            var s = "a x b x c x d ";
            var exp = new[] { "a", "b", "c", "d" };
            var o = s.FromDelimitedString(" x ");
            o.Count().ShouldBe(4);
            for (var i = 0; i < o.Count(); i++)
                exp[i].ShouldBe(o.ElementAt(i));
        }

        [Fact]
        public void ToDelimitedString_ReturnsNullOnNullCollection()
        {
            string[] list = null;
            list.ToDelimitedString().ShouldBeNull();
        }
        [Fact]
        public void ToDelimitedString_ReturnsEmptyStringOnEmptyCollection()
        {
            var list = Array.Empty<string>();
            list.ToDelimitedString().ShouldBe(string.Empty);
        }
        [Fact]
        public void ToDelimitedString_SingleItemArray()
        {
            var list = new[] { "a" };
            list.ToDelimitedString(null).ShouldBe("a");
        }
        [Fact]
        public void ToDelimitedString_NullDelimiter()
        {
            var list = new[] { "a", "b", "c", "d" };
            list.ToDelimitedString(null).ShouldBe("abcd");
        }
        [Fact]
        public void ToDelimitedString_EmptyDelimiter()
        {
            var list = new[] { "a", "b", "c", "d" };
            list.ToDelimitedString().ShouldBe("abcd");
        }
        [Theory]
        [InlineData(" ")]
        [InlineData(" x ")]
        public void ToDelimitedString_NotNullDelimiter(string delimiter)
        {
            var list = new[] { "a", "b", "c", "d" };
            list.ToDelimitedString(delimiter).ShouldBe("a" + delimiter + "b" + delimiter + "c" + delimiter + "d");
        }
    }
}
