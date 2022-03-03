namespace IdentityServer4.Anonymous
{
    public class Constants
    {
        public const string AnonymousFlowEventCategory = "Anonymous";
        public const string AnonymousGrantType = "anonymous";
        public const string AnonymousAuthenticationType = "anonymous";
        public class FormParameters
        {
            public const string Transport = "transport";
            public const string Provider = "provider";
            public const string TransportData = "transport_data";
            public const string State = "state";
            public const string RedirectUri = "redirect_uri";
        }
        public class ClientProperties
        {
            public const string Transports = "transports";
            public const string TransportName = "name";
            public const string TransportProvider = "provider";
            public const string AllowedRetries = "allowed_retries";
            public const string Lifetime = "lifetime";
            public const string UserCodeEmailFormat = "formats:user_code";
            public const string UserCodeSmsFormat = "formats:user_code";
        }
        public sealed class Formats
        {
            public sealed class Fields
            {
                public const string UserCode = "{{USER_CODE}}";
            }
            public sealed class Messages
            {
                public const string UserCodeSmsFormat = "User code is: {{USER_CODE}}";
                public const string UserCodeEmailFormat = "User code is: {{USER_CODE}}";
            }
        }
        public sealed class EndpointNames
        {
            public const string AnonymousAuthorization = "anonymous_endpoint";
        }
        public sealed class EndpointPaths
        {
            public const string AuthorizationEndpoint = "/connect/anonymous";
            public const string VerificationUri = "/anonymous";
        }

        public sealed class Events
        {
            private const int AnonymousEventsStart = 10000;
            public const int AuthorizationSuccessEventId = AnonymousEventsStart + 0;
            public const int AuthorizationFailureEventId = AnonymousEventsStart + 1;
            public const string GrantSuccessEventName = "anonymous-grant-success";
            public const int GrantSuccessEventId = AnonymousEventsStart + 10;
            public const string GrantFailedEventName = "anonymous-grant-failed";
            public const int GrantFailedEventId = AnonymousEventsStart + 11;
        }
        public sealed class UserInteraction
        {
            public const string VerificationCode = "verification_code";
            public const string UserCode = "user_code";
        }
        public sealed class TransportTypes
        {
            public const string Sms = "sms";
            public const string Email = "email";
        }
    }
}
