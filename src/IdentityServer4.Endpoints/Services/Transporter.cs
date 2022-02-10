using System;
using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Services
{
    public interface ITransport
    {
        Func<UserCodeTransportContext, Task<bool>> ShouldHandle { get; }
        Task Transport(UserCodeTransportContext context);
    }
}
