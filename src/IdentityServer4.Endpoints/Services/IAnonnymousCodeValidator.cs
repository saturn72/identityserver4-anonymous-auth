using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Services
{
    public interface IAnonnymousCodeValidator
    {
        Task<AnonnymousCodeValidationResult> ValidateAuthorizedPhoneCodeAsync(AnonnymousCodeValidationRequest request);
        Task<AnonnymousCodeValidationResult> ValidateVerifiedPhoneCodeAsync(AnonnymousCodeValidationRequest request);
    }
}
