using RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static void Warmup(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;
            _ = services.GetService<IOptions<RabbitMQOptions>>().Value;

            var connections = services.GetServices<RabbitMQPersistentConnection>();
            //foreach (var c in connections)
            //{
            //    if (!c.TryConnect())
            //        throw new InvalidOperationException("Failed to start rabbitmq connection. Check log for more details");
            //}
        }
    }
}
