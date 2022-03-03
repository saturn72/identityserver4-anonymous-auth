using IdentityServer4.Anonymous.Services.Generators;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.Anonymous.Tests.Services.Generators
{
    public class DynamicNumericUserCodeGeneratorTests
    {
        [Theory]
        [InlineData(6)]
        [InlineData(10)]
        public async Task GenerateAsync_RequiredLength(int length)
        {
            var g = new DynamicNumericUserCodeGenerator(length);
            var uc = await g.GenerateAsync();
            uc.Length.ShouldBe(length);
        }
    }
}
