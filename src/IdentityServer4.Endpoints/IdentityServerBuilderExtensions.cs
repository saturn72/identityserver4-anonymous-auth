using IdentityServer4.PhoneAuthorizationEndpoint;
using IdentityServer4.PhoneAuthorizationEndpoint.ResponseHandlers;
using IdentityServer4.PhoneAuthorizationEndpoint.Services;
using IdentityServer4.PhoneAuthorizationEndpoint.Services.CodeGenerators;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddPhoneAuthorization(
            this IIdentityServerBuilder builder,
            IConfiguration configuration)
        {
            builder.AddExtensionGrantValidator<PhoneExtensionGrantValidator>();
            builder.AddEndpoint<PhoneAuthorizationEndpointHandler>(
                Constants.EndpointNames.PhoneAuthorization,
                EnsureLeadingSlash(Constants.EndpointPaths.PhoneAuthorization));

            var services = builder.Services;
            services.AddTransient<IPhoneAuthorizationRequestValidator, PhoneAuthorizationRequestValidator>();
            services.AddTransient<IPhoneAuthorizationResponseGenerator, PhoneAuthorizationResponseGenerator>();
            services.AddTransient<IPhoneCodeService, PhoneCodeService>();
            services.AddTransient<IPhoneCodeValidator, PhoneCodeValidator>();
            services.AddSingleton<IUserCodeGenerator>(sp => new DynamicNumericUserCodeGenerator(Defaults.CodeGenetar.NumberOfFigures, Defaults.CodeGenetar.UserCodeType));
            services.AddOptions<PhoneAuthorizationOptions>()
                .Bind(configuration.GetSection(PhoneAuthorizationOptions.Section))
                .Validate(PhoneAuthorizationOptions.Validate);


            return builder;

            static PathString EnsureLeadingSlash(string path)
            {
                if (path != default && !path.StartsWith("/")) path = "/" + path;
                return path;
            }
        }
    }
}
