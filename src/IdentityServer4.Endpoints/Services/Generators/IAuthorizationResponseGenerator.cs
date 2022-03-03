using IdentityServer4.Anonymous.Validation;
using System.Threading.Tasks;

namespace IdentityServer4.Anonymous.Services.Generators
{
    public interface IAuthorizationResponseGenerator
    {
        Task<AuthorizationResponse> ProcessAsync(AuthorizationRequestValidationResult validationResult);
    }
}