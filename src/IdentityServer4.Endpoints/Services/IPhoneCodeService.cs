using System;
using System.Threading.Tasks;

namespace IdentityServer4.PhoneAuthorizationEndpoint.Services
{
    public interface IPhoneCodeService
    {
        Task<PhoneCode> FindByUserCodeAsync(string userCode, bool showExpired = false);
        Task<PhoneCode> FindByPhoneCodeAsync(string code, bool showExpired = false);
        Task UpdateVerificationRetryAsync(Guid id, string provider, string subjectId);
        Task StorePhoneAuthorizationAsync(string userCode, string PhoneCode, PhoneCode data);
        Task UpdateVerificationApprovedAsync(Guid id, PhoneCode data);
    }
}
