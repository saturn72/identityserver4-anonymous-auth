using System.Threading.Tasks;

namespace IdentityServer4.Anonymous.Services
{
    public interface IAnonymousCodeValidator
    {
        Task<AnonymousCodeValidationResult> ValidateAuthorizedPhoneCodeAsync(AnonymousCodeValidationRequest request);
        Task<AnonymousCodeValidationResult> ValidateVerifiedPhoneCodeAsync(AnonymousCodeValidationRequest request);
    }
}
