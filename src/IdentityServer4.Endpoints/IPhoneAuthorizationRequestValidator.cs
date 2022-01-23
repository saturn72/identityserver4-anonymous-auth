using IdentityServer4.PhoneAuthorizationEndpoint.Validation;
using IdentityServer4.Validation;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace IdentityServer4.PhoneAuthorizationEndpoint
{
    public interface IPhoneAuthorizationRequestValidator
    {
        Task<PhoneAuthorizationRequestValidationResult> ValidateAsync(NameValueCollection parameters, ClientSecretValidationResult clientValidationResult);
    }
}