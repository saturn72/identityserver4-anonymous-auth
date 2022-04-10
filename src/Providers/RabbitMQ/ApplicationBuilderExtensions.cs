using Microsoft.Extensions.DependencyInjection;
using RabbitMQ;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static void InitRabbitMQProviders(this IApplicationBuilder app)
        {
            throw new System.NotImplementedException();
            var services = app.ApplicationServices;
            var options = services.GetRequiredService<RabbitMQOptions>();
            foreach (var c in options.Connections)

            {

            }
        }
    }
}
