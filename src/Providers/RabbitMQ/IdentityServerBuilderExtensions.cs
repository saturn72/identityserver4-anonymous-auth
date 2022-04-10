using IdentityServer4.Anonymous.Transport;
using Microsoft.Extensions.Configuration;
using RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddRabbitMQProviders(
            this IIdentityServerBuilder builder,
            IConfiguration configuration)
        {
            var services = builder.Services;

            services.AddOptions<RabbitMQOptions>()
                .Bind(configuration.GetSection(RabbitMQOptions.Section))
                .Validate(RabbitMQOptions.Validate);

            services.AddScoped<ITransporter, RabbitMQTransport>();

            return builder;
        }
    }
}
