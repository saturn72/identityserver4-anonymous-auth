using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Services
{
    public class AnonnymousCodeValidator : IAnonnymousCodeValidator
    {
        public Task<AnonnymousCodeValidationResult> ValidateAuthorizedPhoneCodeAsync(AnonnymousCodeValidationRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<AnonnymousCodeValidationResult> ValidateVerifiedPhoneCodeAsync(AnonnymousCodeValidationRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
