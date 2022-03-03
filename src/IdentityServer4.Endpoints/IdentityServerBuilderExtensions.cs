using IdentityServer4.Anonymous;
using IdentityServer4.Anonymous.Services;
using IdentityServer4.Anonymous.Services.Generators;
using IdentityServer4.Services;
using System;
using Microsoft.Extensions.Configuration;
using IdentityServer4.Anonymous.Stores;
using System.Data.SqlClient;
using IdentityServer4.Anonymous.Endpoints;
using System.Data;
using IdentityServer4.Anonymous.Validation;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddAnonymousAuthorization(
            this IIdentityServerBuilder builder,
            IConfiguration configuration,
            string connectionString)
        {
            builder.AddExtensionGrantValidator<AnonymousExtensionGrantValidator>();
            builder.AddEndpoint<AnonymousAuthorizationEndpoint>(
                Constants.EndpointNames.AnonymousAuthorization,
                Constants.EndpointPaths.AuthorizationEndpoint.EnsureLeadingSlash());

            var services = builder.Services;
            services.AddTransient<IAnonymousAuthorizationRequestValidator, AnonymousAuthorizationRequestValidator>();
            services.AddTransient<IAuthorizationResponseGenerator, AuthorizationResponseGenerator>();
            services.AddTransient<IAnonymousCodeValidator, AnonymousCodeValidator>();
            services.AddSingleton<IUserCodeGenerator>(sp => new DynamicNumericUserCodeGenerator(Defaults.CodeGenetar.NumberOfFigures, Defaults.CodeGenetar.UserCodeType));
            services.AddOptions<AnonymousAuthorizationOptions>()
                .Bind(configuration.GetSection(AnonymousAuthorizationOptions.Section))
                .Validate(AnonymousAuthorizationOptions.Validate);
            services.AddScoped<Func<IDbConnection>>(sp => () => new SqlConnection(connectionString));
            services.AddScoped<IAnonymousCodeStore, DefaultAnonymousCodeStore>();

            services.AddScoped<IRandomStringGenerator, DefaultRandomStringGenerator>();
            services.AddScoped<IAnonymousFlowInteractionService, DefaltAnonymousFlowInteractionService>();
            return builder;
        }
    }
}
