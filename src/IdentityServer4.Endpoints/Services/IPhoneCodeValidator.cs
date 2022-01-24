using System.Threading.Tasks;

namespace IdentityServer4.PhoneAuthorizationEndpoint.Services
{
    public interface IPhoneCodeValidator
    {
        Task<PhoneCodeValidationResult> ValidateAuthorizedPhoneCodeAsync(PhoneCodeValidationRequest request);
        Task<PhoneCodeValidationResult> ValidateVerifiedPhoneCodeAsync(PhoneCodeValidationRequest request);
    }
}
