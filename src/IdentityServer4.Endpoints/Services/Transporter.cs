using System;
using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Services
{
    public interface ITransport
    {
        Func<AnonnymousCodeTransportContext, Task<bool>> ShouldHandle { get; }
        Task Transport(AnonnymousCodeTransportContext context);
    }
}
