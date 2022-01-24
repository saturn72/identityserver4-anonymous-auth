namespace IdentityModel.Client
{
    internal class Constants
    {
        public sealed class PhoneAuthorizationResponse
        {
            public const string PhoneCode = "device_code";
            public const string UserCode = "user_code";
            public const string VerificationUri = "verification_uri";
            public const string VerificationUriComplete = "verification_uri_complete";
            public const string ExpiresIn = "expires_in";
            public const string Interval = "interval";
        }
        public sealed class PhoneAuthorizationRequest
        {
            public const string State = "state";
            public const string Transport = "transport";
            public const string TransportData = "transport_data";
        }
    }
}
