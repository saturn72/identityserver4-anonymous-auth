using System;
using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Transport
{
    public interface ITransporter
    {
        Func<UserCodeTransportContext, Task<bool>> ShouldHandle { get; }
        Task Transport(UserCodeTransportContext context);
    }
}
