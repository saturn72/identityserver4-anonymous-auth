using Shouldly;
using System;
using Xunit;

namespace Identityserver4.Anonymous.Tests.Extensions
{
    public class ObjectExtensionsFunctionsTests
    {
        public class TestClass
        {
            public int Value { get; set; }
        }
        [Fact]
        public void ToJsonString()
        {
            var exp = "{\"value\":123}";
            var t = new TestClass
            {
                Value = 123
            };
            t.ToJsonString().ShouldBe(exp);
        }
    }
}
