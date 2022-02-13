using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Services.Generators
{
    public interface IRandomStringGenerator
    {
        Task<string> Genetare(int length);
    }
}
