using System.Threading.Tasks;

namespace IdentityServer4.PhoneAuthorizationEndpoint.Services
{
    public class PhoneCodeValidator : IPhoneCodeValidator
    {
        public Task<PhoneCodeValidationResult> ValidateAuthorizedPhoneCodeAsync(PhoneCodeValidationRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<PhoneCodeValidationResult> ValidateVerifiedPhoneCodeAsync(PhoneCodeValidationRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
