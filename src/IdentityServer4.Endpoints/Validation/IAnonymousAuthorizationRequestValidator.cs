using IdentityServer4.Validation;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace IdentityServer4.Anonymous.Validation
{
    public interface IAnonymousAuthorizationRequestValidator
    {
        Task<AuthorizationRequestValidationResult> ValidateAsync(NameValueCollection parameters, ClientSecretValidationResult clientValidationResult);
    }
}