namespace IdentityModel.Client
{
    public class AnonymousTokenRequest : TokenRequest
    {
        public string VerificationCode { get; set; }
    }
}
