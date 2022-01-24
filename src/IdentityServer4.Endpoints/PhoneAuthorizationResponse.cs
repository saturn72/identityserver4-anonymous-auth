namespace IdentityServer4.PhoneAuthorizationEndpoint
{
    public class PhoneAuthorizationResponse
    {
        public string PhoneCode { get; set; }
        public string UserCode { get; set; }
        public string VerificationUri { get; set; }

        public string VerificationUriComplete { get; set; }
        public int PhoneCodeLifetime { get; set; }
        public int Interval { get; set; }
        public int Retries { get; set; }
    }
}