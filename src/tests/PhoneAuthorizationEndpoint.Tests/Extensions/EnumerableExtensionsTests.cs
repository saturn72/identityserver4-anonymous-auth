using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace PhoneAuthorizationEndpoint.Tests.Extensions
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void IsNullOrEmpty_ReturnsTrue()
        {
            (null as IEnumerable<string>).IsNullOrEmpty().ShouldBeTrue();
            (new string[] { }).IsNullOrEmpty().ShouldBeTrue();
        }
        [Fact]
        public void IsNullOrEmpty_ReturnsFalse()
        {
            (new[] { 1, 2, 3 }).IsNullOrEmpty().ShouldBeFalse();
        }
    }
}
