namespace IdentityModel.Client
{
    public class AnonymousAuthorizationResponse : ProtocolResponse
    {
        public string VerificationCode => Json.TryGetString(Constants.AnonymousAuthorizationResponse.VerificationCode);
        public string VerificationUri => Json.TryGetString(Constants.AnonymousAuthorizationResponse.VerificationUri);
        public string VerificationUriComplete => Json.TryGetString(Constants.AnonymousAuthorizationResponse.VerificationUriComplete);
        public int? ExpiresIn => Json.TryGetInt(Constants.AnonymousAuthorizationResponse.ExpiresIn);
        public int Interval => Json.TryGetInt(Constants.AnonymousAuthorizationResponse.Interval) ?? 5;
        public string ErrorDescription => Json.TryGetString(OidcConstants.TokenResponse.ErrorDescription);
    }
}
