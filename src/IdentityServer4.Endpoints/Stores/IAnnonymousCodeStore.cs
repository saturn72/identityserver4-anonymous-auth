using IdentityServer4.Anonnymous.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Stores
{
    public interface IAnnonymousCodeStore
    {
        Task<AnonnymousCodeInfo> FindByUserCodeAsync(string code, bool returnExpired);
        Task<AnonnymousCodeInfo> FindByVerificationCodeAsync(string code, bool returnExpired);
        Task<AnonnymousCodeInfo> FindByVerificationCodeAndUserCodeAsync(string verificationCode, string userCode);
        Task StoreAnonnymousCodeInfoAsync(string verificationCode, AnonnymousCodeInfo data);
        Task UpdateVerificationRetryAsync(string verificationCode);
        Task<IEnumerable<string>> GetAllSubjectIds();
        Task Authorize(AnonnymousCodeInfo code);
    }
}
