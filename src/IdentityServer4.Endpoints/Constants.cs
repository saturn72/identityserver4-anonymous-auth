namespace IdentityServer4.PhoneAuthorizationEndpoint
{
    internal class Constants
    {
        public class FormParameters
        {
            public const string Transport = "transport";
        }
        public const string PhoneAuthorizationEndpointName = "Phone";
        public const string PhoneFlowEventCategory = "Phone";
        public const string PhoneFlowGrantType = "phone";

        private const int DeviceFlowEventsStart = 10000;
        public const int PhoneAuthorizationSuccessEventId = DeviceFlowEventsStart + 0;
        public const int PhoneAuthorizationFailureEventId = DeviceFlowEventsStart + 1;
    }
}
