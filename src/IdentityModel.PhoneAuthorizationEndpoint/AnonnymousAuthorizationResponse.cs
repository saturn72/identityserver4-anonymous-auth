namespace IdentityModel.Client
{
    public class AnonnymousAuthorizationResponse : ProtocolResponse
    {
        public string VerificationCode => Json.TryGetString(Constants.AnonnymousAuthorizationResponse.VerificationCode);
        public string VerificationUri => Json.TryGetString(Constants.AnonnymousAuthorizationResponse.VerificationUri);
        public string VerificationUriComplete => Json.TryGetString(Constants.AnonnymousAuthorizationResponse.VerificationUriComplete);
        public int? ExpiresIn => Json.TryGetInt(Constants.AnonnymousAuthorizationResponse.ExpiresIn);
        public int Interval => Json.TryGetInt(Constants.AnonnymousAuthorizationResponse.Interval) ?? 5;
        public string ErrorDescription => Json.TryGetString(OidcConstants.TokenResponse.ErrorDescription);
    }
}
