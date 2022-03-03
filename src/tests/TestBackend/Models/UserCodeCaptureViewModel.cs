namespace IdentityServer4.Anonymous.UI.Models
{
    public record UserCodeCaptureViewModel
    {
        public string Transport { get; set; }
        public string UserCode { get; set; }
        public string VerificationCode { get; set; }
    }
}
