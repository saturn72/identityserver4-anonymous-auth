using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Anonnymous.Transport
{
    public static class TransporterExtensions
    {
        public static Task Transport(this IEnumerable<ITransporter> transports, UserCodeTransportContext context)
        {
            var handlers = new List<(ITransporter transport, Task<bool> shouldHandle)>();
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
