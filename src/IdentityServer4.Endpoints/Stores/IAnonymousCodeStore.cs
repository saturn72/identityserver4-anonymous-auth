using IdentityServer4.Anonymous.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer4.Anonymous.Stores
{
    public interface IAnonymousCodeStore
    {
        Task<AnonymousCodeInfo> FindByUserCodeAsync(string code, bool returnExpired);
        Task<AnonymousCodeInfo> FindByVerificationCodeAsync(string code, bool returnExpired);
        Task<AnonymousCodeInfo> FindByVerificationCodeAndUserCodeAsync(string verificationCode, string userCode);
        Task StoreAnonymousCodeInfoAsync(string verificationCode, AnonymousCodeInfo data);
        Task UpdateVerificationRetryAsync(string verificationCode);
        Task<IEnumerable<string>> GetAllSubjectIdsByClientIdAsync(string clientId);
        Task UpdateAuthorization(AnonymousCodeInfo code);
    }
}
