using IdentityServer4.Configuration;
using System;

namespace IdentityServer4.PhoneAuthorizationEndpoint
{
    public class PhoneAuthorizationOptions
    {
        public const string Section = "phoneAuth";
        public int AllowedRetries { get; set; } = Defaults.AllowedRetries;
        public string AllowedRetriesPropertyName { get; set; } = Constants.ClientProperties.AllowedRetries;
        public InputLengthRestrictions InputLengthRestrictions { get; set; } = new InputLengthRestrictions();
        public int DefaultInteractionCodeLifetime { get; set; } = Defaults.CodeLifetime;
        public string DefaultUserCodeType { get; set; } = Defaults.CodeGenetar.UserCodeType;
        public int Interval { get; set; } = Defaults.Interval;
        public string LifetimePropertyName { get; set; } = Constants.ClientProperties.Lifetime;
        public string PhoneVerificationUrl { get; set; } = Constants.EndpointPaths.PhoneVerification;
        public string[] TransportOptions { get; set; } = Array.Empty<string>();

        internal static bool Validate(PhoneAuthorizationOptions options)
        {
            if (options.AllowedRetries == default)
                throw new ArgumentException($"bad or missinf config: {nameof(AllowedRetries)}");

            if (!options.AllowedRetriesPropertyName.HasValue())
                throw new ArgumentException($"bad or missinf config: {nameof(AllowedRetriesPropertyName)}");

            if (options.DefaultInteractionCodeLifetime == default)
                throw new ArgumentException($"bad or missinf config: {nameof(DefaultInteractionCodeLifetime)}");

            if (options.DefaultUserCodeType.HasValue())
                throw new ArgumentException($"bad or missinf config: {nameof(DefaultUserCodeType)}");

            if (options.InputLengthRestrictions == default)
                throw new ArgumentException($"bad or missinf config: {nameof(InputLengthRestrictions)}");

            if (options.Interval == default)
                throw new ArgumentException($"bad or missinf config: {nameof(Interval)}");

            if (!options.LifetimePropertyName.HasValue())
                throw new ArgumentException($"bad or missinf config: {nameof(LifetimePropertyName)}");

            if (options.PhoneVerificationUrl == default)
                throw new ArgumentException($"bad or missinf config: {nameof(PhoneVerificationUrl)}");

            return true;
        }
    }
}
