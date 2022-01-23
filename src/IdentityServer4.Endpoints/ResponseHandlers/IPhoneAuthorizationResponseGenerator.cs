using IdentityServer4.PhoneAuthorizationEndpoint.Validation;
using System.Threading.Tasks;

namespace IdentityServer4.PhoneAuthorizationEndpoint.ResponseHandlers
{
    public interface IPhoneAuthorizationResponseGenerator
    {
        Task<PhoneAuthorizationResponse> ProcessAsync(PhoneAuthorizationRequestValidationResult validationResult, string baseUrl);
    }
}