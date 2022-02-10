using IdentityServer4.Anonnymous.Services;
using System;
using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Data
{
    public interface IAnnonymousCodeStore
    {
        Task<AnonnymousCodeInfo> FindByUserCodeAsync(string code, bool includeExpiredAndVerified);
        Task<AnonnymousCodeInfo> FindByAnonnymousCodeAsync(string code, bool returnExpired);
        Task StoreAnonnymousCodeInfoAsync(string userCode, AnonnymousCodeInfo data);
        Task Activate(Guid id, string userCode);
    }
}
