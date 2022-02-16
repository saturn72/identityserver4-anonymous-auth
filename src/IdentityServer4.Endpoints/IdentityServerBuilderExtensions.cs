using IdentityServer4.Anonnymous;
using IdentityServer4.Anonnymous.Services;
using IdentityServer4.Anonnymous.Services.Generators;
using IdentityServer4.Services;
using System;
using Microsoft.Extensions.Configuration;
using IdentityServer4.Anonnymous.Stores;
using System.Data.SqlClient;
using IdentityServer4.Anonnymous.Endpoints;
using System.Data;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddAnonnymousAuthorization(
            this IIdentityServerBuilder builder,
            IConfiguration configuration,
            string connectionString)
        {
            builder.AddExtensionGrantValidator<AnonnymousExtensionGrantValidator>();
            builder.AddEndpoint<AnonnymousAuthorizationEndpoint>(
                Constants.EndpointNames.AnonnymousAuthorization,
                Constants.EndpointPaths.AuthorizationEndpoint.EnsureLeadingSlash());

            var services = builder.Services;
            services.AddTransient<IAnonnymousAuthorizationRequestValidator, AnonnymousAuthorizationRequestValidator>();
            services.AddTransient<IAuthorizationResponseGenerator, AuthorizationResponseGenerator>();
            services.AddTransient<IAnonnymousCodeValidator, AnonnymousCodeValidator>();
            services.AddSingleton<IUserCodeGenerator>(sp => new DynamicNumericUserCodeGenerator(Defaults.CodeGenetar.NumberOfFigures, Defaults.CodeGenetar.UserCodeType));
            services.AddOptions<AnonnymousAuthorizationOptions>()
                .Bind(configuration.GetSection(AnonnymousAuthorizationOptions.Section))
                .Validate(AnonnymousAuthorizationOptions.Validate);
            services.AddScoped<Func<IDbConnection>>(sp => () => new SqlConnection(connectionString));
            services.AddScoped<IAnnonymousCodeStore, DefaultAnnonymousCodeStore>();

            services.AddScoped<IRandomStringGenerator, DefaultRandomStringGenerator>();
            services.AddScoped<IAnonnymousFlowInteractionService, DefaltAnonnymousFlowInteractionService>();
            return builder;
        }
    }
}
