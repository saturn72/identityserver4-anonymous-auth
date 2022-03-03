using System.Threading.Tasks;

namespace IdentityServer4.Anonymous.Services
{
    public class AnonymousCodeValidator : IAnonymousCodeValidator
    {
        public Task<AnonymousCodeValidationResult> ValidateAuthorizedPhoneCodeAsync(AnonymousCodeValidationRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<AnonymousCodeValidationResult> ValidateVerifiedPhoneCodeAsync(AnonymousCodeValidationRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
