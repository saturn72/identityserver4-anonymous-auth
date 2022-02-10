namespace IdentityServer4.Anonnymous.Services
{
    public record AnonnymousCodeTransportContext
    {
        public string Body { get; set; }
        public string Data { get; set; }
        public string Provider { get; set; }
        public string Transport { get; set; }
        public string VerificationUri { get; set; }
    }
}