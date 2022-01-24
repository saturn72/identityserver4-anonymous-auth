namespace IdentityServer4.PhoneAuthorizationEndpoint
{
    public class Constants
    {
        public class FormParameters
        {
            public const string Transport = "transport";
            public const string TransportData = "transport_data";
            public const string State = "state";
        }
        public class ClientProperties
        {
            public const string Transports = "transports";
            public const string TransportName = "name";
            public const string AllowedRetries = "allowedRetries";
            public const string AllowUserCodeTransfer = "allowUserCodeTransfer";
            public const string Lifetime = "lifetime";
        }
        internal sealed class EndpointNames
        {
            internal const string PhoneAuthorization = "phone_endpoint";
        }
        internal sealed class EndpointPaths
        {
            public const string PhoneAuthorization = "/connect/phone";
            public const string PhoneVerification = "/phone";
        }
        public const string PhoneFlowEventCategory = "Phone";
        public const string PhoneGrantType = "phone";

        public sealed class Events
        {
            private const int DeviceFlowEventsStart = 10000;
            public const int PhoneAuthorizationSuccessEventId = DeviceFlowEventsStart + 0;
            public const int PhoneAuthorizationFailureEventId = DeviceFlowEventsStart + 1;
        }
    }
}
