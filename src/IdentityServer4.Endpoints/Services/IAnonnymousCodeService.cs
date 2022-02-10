using System;
using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Services
{
    public interface IAnonnymousCodeService
    {
        Task<AnonnymousCodeInfo> FindByUserCodeAsync(string code, bool showExpired = false);
        Task<AnonnymousCodeInfo> FindByVerificationCodeAsync(string code, bool showExpired = false);
        Task<AnonnymousCodeInfo> FindByVerificationCodeAndUserCodeAsync(string verificationCode, string userCode);
        Task UpdateVerificationRetryAsync(string verificationCode);
        Task StoreAnonnymousCodeInfoAsync(string userCode, AnonnymousCodeInfo data);
    }
}
