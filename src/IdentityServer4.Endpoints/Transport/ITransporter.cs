using System;
using System.Threading.Tasks;

namespace IdentityServer4.Anonymous.Transport
{
    public interface ITransporter
    {
        Func<UserCodeTransportContext, Task<bool>> ShouldHandle { get; }
        Task Transport(UserCodeTransportContext context);
    }
}
