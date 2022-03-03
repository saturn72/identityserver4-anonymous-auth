using System.Threading.Tasks;

namespace IdentityServer4.Anonymous.Services.Generators
{
    public interface IRandomStringGenerator
    {
        Task<string> Genetare(int length);
    }
}
