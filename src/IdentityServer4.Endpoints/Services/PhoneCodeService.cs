using System;
using System.Threading.Tasks;

namespace IdentityServer4.PhoneAuthorizationEndpoint.Services
{
    public class PhoneCodeService : IPhoneCodeService
    {
        public Task<PhoneCode> FindByPhoneCodeAsync(string code, bool showExpired = false)
        {
            throw new NotImplementedException();
        }

        public Task<PhoneCode> FindByUserCodeAsync(string userCode, bool showExpired = false)
        {
            throw new NotImplementedException();
        }

        public Task StorePhoneAuthorizationAsync(string userCode, string PhoneCode, PhoneCode data)
        {
            throw new NotImplementedException();
        }

        public Task UpdateVerificationApprovedAsync(Guid id, PhoneCode data)
        {
            throw new NotImplementedException();
        }

        public Task UpdateVerificationRetryAsync(Guid id, string provider, string subjectId)
        {
            throw new NotImplementedException();
        }
    }
}
