using IdentityServer4.PhoneAuthorizationEndpoint.Validation;
using System.Threading.Tasks;

namespace IdentityServer4.PhoneAuthorizationEndpoint.ResponseHandlers
{
    public class PhoneAuthorizationResponseGenerator : IPhoneAuthorizationResponseGenerator
    {
        public Task<PhoneAuthorizationResponse> ProcessAsync(PhoneAuthorizationRequestValidationResult validationResult, string baseUrl)
        {
            throw new System.NotImplementedException();
        }
    }

}