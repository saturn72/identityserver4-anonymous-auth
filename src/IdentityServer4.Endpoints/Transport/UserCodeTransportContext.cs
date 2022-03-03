namespace IdentityServer4.Anonymous.Transport
{
    public record UserCodeTransportContext
    {
        public string Body { get; set; }
        public string Data { get; set; }
        public string Provider { get; set; }
        public string Transport { get; set; }
    }
}