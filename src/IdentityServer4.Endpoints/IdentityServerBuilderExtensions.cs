using IdentityServer4.PhoneAuthorizationEndpoint;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddPhoneAuthorization(this IIdentityServerBuilder builder)
        {

            builder.AddEndpoint<PhoneAuthorizationEndpointHandler>(Constants.PhoneAuthorizationEndpointName, Constants.PhoneAuthorizationEndpointName);
            builder.Services.AddSingleton<IPhoneAuthorizationRequestValidator, PhoneAuthorizationRequestValidator>();

            return builder;
        }
    }
}
