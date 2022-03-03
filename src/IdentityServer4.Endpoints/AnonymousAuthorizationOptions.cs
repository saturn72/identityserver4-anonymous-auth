using IdentityServer4.Configuration;
using System;

namespace IdentityServer4.Anonymous
{
    public class AnonymousAuthorizationOptions
    {
        public const string Section = "anonymous";
        public int AllowedRetries { get; set; } = Defaults.AllowedRetries;
        public string AllowedRetriesPropertyName { get; set; } = Constants.ClientProperties.AllowedRetries;
        public InputLengthRestrictions InputLengthRestrictions { get; set; } = new InputLengthRestrictions();
        public int DefaultLifetime { get; set; } = Defaults.CodeLifetime;
        public string DefaultUserCodeType { get; set; } = Defaults.CodeGenetar.UserCodeType;
        public string DefaultUserCodeSmsFormat { get; set; } = Constants.Formats.Messages.UserCodeSmsFormat;
        public string DefaultUserCodeEmailFormat { get; set; } = Constants.Formats.Messages.UserCodeEmailFormat;
        public string DefaultUserCodeEmailFormatPropertyName { get; set; } = Constants.ClientProperties.UserCodeEmailFormat;
        public string DefaultUserCodeSmSFormatPropertyName { get; set; } = Constants.ClientProperties.UserCodeSmsFormat;
        public int Interval { get; set; } = Defaults.Interval;
        public string LifetimePropertyName { get; set; } = Constants.ClientProperties.Lifetime;
        public string[] Transports { get; set; } = Array.Empty<string>();
        public string VerificationUri { get; set; } = Constants.EndpointPaths.VerificationUri;

        public static bool Validate(AnonymousAuthorizationOptions options)
        {
            if (options.AllowedRetries == default)
                throw new ArgumentException($"bad or missing config: {nameof(AllowedRetries)}");

            if (!options.AllowedRetriesPropertyName.HasValue())
                throw new ArgumentException($"bad or missing config: {nameof(AllowedRetriesPropertyName)}");

            if (options.DefaultLifetime == default)
                throw new ArgumentException($"bad or missing config: {nameof(DefaultLifetime)}");

            if (!options.DefaultUserCodeType.HasValue())
                throw new ArgumentNullException($"bad or missing config: {nameof(DefaultUserCodeType)}");

            if (options.InputLengthRestrictions == default)
                throw new ArgumentException($"bad or missing config: {nameof(InputLengthRestrictions)}");

            if (options.Interval == default)
                throw new ArgumentException($"bad or missing config: {nameof(Interval)}");

            if (!options.LifetimePropertyName.HasValue())
                throw new ArgumentException($"bad or missing config: {nameof(LifetimePropertyName)}");

            return true;
        }
    }
}
