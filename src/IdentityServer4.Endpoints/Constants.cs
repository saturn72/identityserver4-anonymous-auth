namespace IdentityServer4.Anonnymous
{
    public class Constants
    {
        public const string AnonnymousFlowEventCategory = "Anonnymous";
        public const string AnonnymousGrantType = "anonnymous";
        public const string AnonnymousAuthenticationType = "anonnymous";
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
            public const string AnonnymousAuthorization = "anonnymous_endpoint";
        }
        public sealed class EndpointPaths
        {
            public const string AuthorizationEndpoint = "/connect/anonnymous";
            public const string VerificationUri = "/anonnymous/verify";
        }

        public sealed class Events
        {
            private const int AnonnymousEventsStart = 10000;
            public const int AuthorizationSuccessEventId = AnonnymousEventsStart + 0;
            public const int AuthorizationFailureEventId = AnonnymousEventsStart + 1;
            public const string GrantSuccessEventName = "anonnymous-grant-success";
            public const int GrantSuccessEventId = AnonnymousEventsStart + 10;
            public const string GrantFailedEventName = "anonnymous-grant-failed";
            public const int GrantFailedEventId = AnonnymousEventsStart + 11;
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
