using IdentityServer4.Anonnymous.Validation;
using IdentityServer4.Validation;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous
{
    public interface IAnonnymousAuthorizationRequestValidator
    {
        Task<AuthorizationRequestValidationResult> ValidateAsync(NameValueCollection parameters, ClientSecretValidationResult clientValidationResult);
    }
}