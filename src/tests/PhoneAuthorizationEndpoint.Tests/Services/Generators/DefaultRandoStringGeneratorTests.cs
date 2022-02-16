using IdentityServer4.Anonnymous.Services.Generators;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Identityserver4.Anonnymous.Tests.Services.Generators
{
    public class DefaultRandoStringGeneratorTests
    {
        [Theory]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        public async Task GenerateStringInLength(int l)
        {
            var g = new DefaultRandomStringGenerator();
            var t = await g.Genetare(l);
            t.Replace(" ", string.Empty).Length.ShouldBe(l);
        }
    }
}
