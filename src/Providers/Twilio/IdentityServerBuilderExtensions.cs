using IdentityServer4.Anonnymous.Services;
using Microsoft.Extensions.Configuration;
using TwilioProviders;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddTwilioProviders(
            this IIdentityServerBuilder builder,
            IConfiguration configuration)
        {
            var services = builder.Services;


            services.AddOptions<TwilioOptions>()
                .Bind(configuration.GetSection(TwilioOptions.Section))
                .Validate(TwilioOptions.Validate);

            services.AddScoped<ITransport, TwilioSmsTransport>();

            return builder;
        }
    }
}
