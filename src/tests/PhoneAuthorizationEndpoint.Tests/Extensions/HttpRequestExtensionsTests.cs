using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using System;
using Xunit;

namespace IdentityServer4.Anonymous.Tests.Extensions
{
    public class HttpRequestExtensionsTests
    {
        [Fact]
        public void HasApplicationFormContentType_Throws_OnNull()
        {
            Should.Throw<NullReferenceException>(() => HttpRequestExtensions.HasApplicationFormContentType(null));
        }
        [Fact]
        public void HasApplicationFormContentType_False_OnNullContentType()
        {
            var request = new Mock<HttpRequest>();
            HttpRequestExtensions.HasApplicationFormContentType(request.Object).ShouldBeFalse();
        }
        [Theory]
        [InlineData(default)]
        [InlineData("")]
        [InlineData(" ")]
        public void HasApplicationFormContentType_False_OnBlankContentType(string ct)
        {
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.ContentType).Returns(ct);
            HttpRequestExtensions.HasApplicationFormContentType(request.Object).ShouldBeFalse();
        }
        [Fact]
        public void HasApplicationFormContentType_False_OnNonFormContentType()
        {
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.ContentType).Returns("ddd");
            HttpRequestExtensions.HasApplicationFormContentType(request.Object).ShouldBeFalse();
        }

        [Theory]
        [InlineData("application/x-www-form-urlencoded")]
        [InlineData("APPLICATION/X-WWW-FORM-URLENCODED")]
        public void HasApplicationFormContentType_False_OnvalieFormContentTypeCaseInsensitive(string ct)
        {
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.ContentType).Returns(ct);
            HttpRequestExtensions.HasApplicationFormContentType(request.Object).ShouldBeTrue();
        }
    }
}
