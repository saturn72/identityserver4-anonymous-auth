namespace IdentityServer4.PhoneAuthorizationEndpoint
{
    public class Constants
    {
        public class FormParameters
        {
            public const string Transport = "transport";
        }
        public const string PhoneAuthorizationEndpointName = "phone_endpoint";
        public const string PhoneAuthorizationEndpointPath = "/connect/phone";
        public const string PhoneFlowEventCategory = "Phone";
        public const string PhoneGrantType = "phone";

        private const int DeviceFlowEventsStart = 10000;
        public const int PhoneAuthorizationSuccessEventId = DeviceFlowEventsStart + 0;
        public const int PhoneAuthorizationFailureEventId = DeviceFlowEventsStart + 1;
    }
}
