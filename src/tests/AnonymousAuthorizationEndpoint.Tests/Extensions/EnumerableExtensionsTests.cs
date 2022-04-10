using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Identityserver4.Anonymous.Tests.Extensions
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void IsNullOrEmpty_ReturnsTrue()
        {
            (null as IEnumerable<string>).IsNullOrEmpty().ShouldBeTrue();
            (Array.Empty<string>()).IsNullOrEmpty().ShouldBeTrue();
        }
        [Fact]
        public void IsNullOrEmpty_ReturnsFalse()
        {
            (new[] { 1, 2, 3 }).IsNullOrEmpty().ShouldBeFalse();
        }
    }
}
