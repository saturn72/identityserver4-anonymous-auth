namespace IdentityServer4.Anonnymous.Services
{
    public record UserCodeTransportContext
    {
        public string Body { get; set; }
        public string Data { get; set; }
        public string Provider { get; set; }
        public string Transport { get; set; }
    }
}