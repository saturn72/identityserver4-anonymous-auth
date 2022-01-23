using IdentityServer4.Configuration;

namespace IdentityServer4.PhoneAuthorizationEndpoint
{
    public class PhoneAuthorizationOptions
    {
        public InputLengthRestrictions InputLengthRestrictions { get; set; }
        public string[] TransportOptions { get; set; }
    }
}
