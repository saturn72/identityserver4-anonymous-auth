namespace IdentityServer4.Anonnymous
{
    public class Constants
    {
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
            public const string AnonnymousAuthorizationEndpoint = "/connect/anonnymous";
            public const string ActivationEndpoint = AnonnymousAuthorizationEndpoint + "/activate";
            public const string VerificationEndpoint = AnonnymousAuthorizationEndpoint + "/verify";
        }
        public const string AnonnymousFlowEventCategory = "Anonnymous";
        public const string AnonnymousGrantType = "anonnymous";

        public sealed class Events
        {
            private const int DeviceFlowEventsStart = 10000;
            public const int AuthorizationSuccessEventId = DeviceFlowEventsStart + 0;
            public const int AuthorizationFailureEventId = DeviceFlowEventsStart + 1;
        }
        public sealed class IdentityModel
        {
            public const string AnonnymousCode = "anonnymous_code";
        }
        public sealed class TransportTypes
        {
            public const string Sms = "sms";
            public const string Email = "email";

        }
    }
}
