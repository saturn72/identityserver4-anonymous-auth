using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Services
{
    internal static class AnonnymousCodeTransportExtensions
    {
        public static Task Transport(this IEnumerable<ITransport> transports, AnonnymousCodeTransportContext context)
        {
            var handlers = new List<(ITransport transport, Task<bool> shouldHandle)>();
            foreach (var t in transports)
                handlers.Add((t, t.ShouldHandle(context)));
            var tasks = handlers.Select(c => c.shouldHandle).ToArray();
            Task.WaitAll(tasks);

            foreach (var (transport, _) in handlers.Where(h => h.shouldHandle.Result))
                _ = transport.Transport(context);

            return Task.CompletedTask;
        }
    }
}
