using IdentityServer4.Anonnymous.Services.Generators;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.Anonnymous.Tests.Services.Generators
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
