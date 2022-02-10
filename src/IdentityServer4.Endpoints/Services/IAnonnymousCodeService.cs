using System;
using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Services
{
    public interface IAnonnymousCodeService
    {
        Task<AnonnymousCodeInfo> FindByUserCodeAsync(string userCode, bool showExpired = false);
        Task<AnonnymousCodeInfo> FindByAnonnymousCodeAsync(string code, bool showExpired = false);
        Task UpdateVerificationRetryAsync(Guid id, string provider, string subjectId);
        Task StoreAnonnymousCodeInfoAsync(string userCode, AnonnymousCodeInfo data);
        Task UpdateVerificationApprovedAsync(Guid id, AnonnymousCodeInfo data);
        Task Activate(Guid id, string code);
    }
}
