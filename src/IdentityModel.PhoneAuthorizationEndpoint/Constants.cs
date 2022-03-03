namespace IdentityModel.Client
{
    internal class Constants
    {
        internal const string GrantType = "anonymous";
        public sealed class AnonymousAuthorizationResponse
        {
            public const string VerificationCode = "verification_code";
            public const string VerificationUri = "verification_uri";
            public const string VerificationUriComplete = "verification_uri_complete";
            public const string ExpiresIn = "expires_in";
            public const string Interval = "interval";
        }
        public sealed class AnonymousAuthorizationRequest
        {
            public const string RedirectUri = "redirect_uri";
            public const string State = "state";
            public const string Provider = "provider";
            public const string Transport = "transport";
            public const string TransportData = "transport_data";
        }
    }
}
