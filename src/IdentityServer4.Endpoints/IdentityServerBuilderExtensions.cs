using IdentityServer4.Anonnymous;
using IdentityServer4.Anonnymous.ResponseHandling;
using IdentityServer4.Anonnymous.Services;
using IdentityServer4.Anonnymous.Services.CodeGenerators;
using IdentityServer4.Services;
using System;
using Microsoft.Extensions.Configuration;
using IdentityServer4.Anonnymous.Data;
using System.Data.SqlClient;
using IdentityServer4.Anonnymous.Endpoints;

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
            builder.AddEndpoint<AnonnymousAuthorizationEndpointHandler>(
                Constants.EndpointNames.AnonnymousAuthorization,
                Constants.EndpointPaths.AnonnymousAuthorizationEndpoint.EnsureLeadingSlash());

            builder.AddEndpoint<AnonnymousActivationEndpointHandler>(
                Constants.EndpointNames.AnonnymousAuthorization,
                Constants.EndpointPaths.ActivationEndpoint.EnsureLeadingSlash());

            builder.AddEndpoint<AnonnymousVerificationEndpointHandler>(
                Constants.EndpointNames.AnonnymousAuthorization,
                Constants.EndpointPaths.VerificationEndpoint.EnsureLeadingSlash());

            var services = builder.Services;
            services.AddTransient<IAnonnymousAuthorizationRequestValidator, AnonnymousAuthorizationRequestValidator>();
            services.AddTransient<IAuthorizationResponseGenerator, AnonnymousAuthorizationResponseGenerator>();
            services.AddTransient<IAnonnymousCodeService, AnonnymousCodeService>();
            services.AddTransient<IAnonnymousCodeValidator, AnonnymousCodeValidator>();
            services.AddSingleton<IUserCodeGenerator>(sp => new DynamicNumericUserCodeGenerator(Defaults.CodeGenetar.NumberOfFigures, Defaults.CodeGenetar.UserCodeType));
            services.AddOptions<AnonnymousAuthorizationOptions>()
                .Bind(configuration.GetSection(AnonnymousAuthorizationOptions.Section))
                .Validate(AnonnymousAuthorizationOptions.Validate);

            services.AddScoped<IAnnonymousCodeStore>(sp => new DapperAnnonymousCodeStore(() => new SqlConnection(connectionString)));

            return builder;
        }
    }
}
