namespace IdentityServer4.Anonnymous.ResponseHandling
{
    public class AuthorizationResponse
    {
        public string AnonnymousCode { get; set; }
        public string UserCode { get; set; }
        public string ActivationUri { get; set; }
        public string ActivationUriComplete { get; set; }
        public int AnonnymousCodeLifetime { get; set; }
        public int Interval { get; set; }
        public int Retries { get; set; }
    }
}