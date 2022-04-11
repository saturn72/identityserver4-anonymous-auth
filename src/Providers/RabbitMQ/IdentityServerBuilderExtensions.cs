using IdentityServer4.Anonymous.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddRabbitMQProviders(
            this IIdentityServerBuilder builder,
            IConfiguration configuration)
        {
            var services = builder.Services;


            var section = configuration.GetSection(RabbitMQOptions.Section);
            var options = new RabbitMQOptions();
            section.Bind(options);
            RabbitMQOptions.Validate(options);
            _ = options.Connections.Select(ci => services.AddSingleton(sp => new RabbitMQPersistentConnection(ci, sp.GetService<ILogger<RabbitMQPersistentConnection>>()))).ToArray();

            services.AddScoped<ITransporter, RabbitMQTransport>();
            services.AddSingleton<RabbitMQConnectionFactory>();

            return builder;
        }
    }
}
