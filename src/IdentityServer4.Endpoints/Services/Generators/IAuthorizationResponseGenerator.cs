using IdentityServer4.Anonnymous.Validation;
using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Services.Generators
{
    public interface IAuthorizationResponseGenerator
    {
        Task<AuthorizationResponse> ProcessAsync(AuthorizationRequestValidationResult validationResult);
    }
}