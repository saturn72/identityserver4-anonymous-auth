using IdentityServer4.Anonnymous.Services;
using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Data
{
    public interface IAnnonymousCodeStore
    {
        Task<AnonnymousCodeInfo> FindByUserCodeAsync(string code, bool returnExpired);
        Task<AnonnymousCodeInfo> FindByVerificationCodeAsync(string code, bool returnExpired);
        Task<AnonnymousCodeInfo> FindByVerificationCodeAndUserCodeAsync(string verificationCode, string userCode);
        Task StoreAnonnymousCodeInfoAsync(string verificationCode, AnonnymousCodeInfo data);
        Task UpdateVerificationRetryAsync(string verificationCode);
    }
}
