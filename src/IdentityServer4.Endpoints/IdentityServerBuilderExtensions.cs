using IdentityServer4.PhoneAuthorizationEndpoint;
using IdentityServer4.PhoneAuthorizationEndpoint.ResponseHandlers;
using Microsoft.AspNetCore.Http;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddPhoneAuthorization(this IIdentityServerBuilder builder)
        {
            builder.AddExtensionGrantValidator<PhoneExtensionGrantValidator>();
            builder.AddEndpoint<PhoneAuthorizationEndpointHandler>(Constants.PhoneAuthorizationEndpointName, EnsureLeadingSlash(Constants.PhoneAuthorizationEndpointPath));

            var services = builder.Services;
            services.AddTransient<IPhoneAuthorizationRequestValidator, PhoneAuthorizationRequestValidator>();
            services.AddTransient<IPhoneAuthorizationResponseGenerator, PhoneAuthorizationResponseGenerator>();


            return builder;

            static PathString EnsureLeadingSlash(string path)
            {
                if (path != default && !path.StartsWith("/")) path = "/" + path;
                return path;
            }
        }
    }
}
