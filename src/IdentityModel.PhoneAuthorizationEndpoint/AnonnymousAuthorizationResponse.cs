namespace IdentityModel.Client
{
    public class AnonnymousAuthorizationResponse : ProtocolResponse
    {
        /// <summary>
        /// Gets the phone verification code.
        /// </summary>
        /// <value>
        /// The phone code.
        /// </value>
        public string AnonnymousCode => Json.TryGetString(Constants.AnonnymousAuthorizationResponse.AnonnymousCode);

        /// <summary>
        /// Gets the end-user verification code.
        /// </summary>
        /// <value>
        /// The user code.
        /// </value>
        public string UserCode => Json.TryGetString(Constants.AnonnymousAuthorizationResponse.UserCode);
        public string VerificationUri => Json.TryGetString(Constants.AnonnymousAuthorizationResponse.VerificationUri);
        public string ActivationUri => Json.TryGetString(Constants.AnonnymousAuthorizationResponse.ActivationUri);

        /// <summary>
        /// Gets the verification URI that includes the "user_code" (or other information with the same function as the "user_code"), designed for non-textual transmission.
        /// </summary>
        /// <value>
        /// The complete verification URI.
        /// </value>
        public string ActivationUriComplete => Json.TryGetString(Constants.AnonnymousAuthorizationResponse.ActivationUriComplete);

        /// <summary>
        /// Gets the lifetime in seconds of the "phone_code" and "user_code".
        /// </summary>
        /// <value>
        /// The expires in.
        /// </value>
        public int? ExpiresIn => Json.TryGetInt(Constants.AnonnymousAuthorizationResponse.ExpiresIn);

        /// <summary>
        /// Gets the minimum amount of time in seconds that the client SHOULD wait between polling requests to the token endpoint. If no value is provided, clients MUST use 5 as the default.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        public int Interval => Json.TryGetInt(Constants.AnonnymousAuthorizationResponse.Interval) ?? 5;

        /// <summary>
        /// Gets the error description.
        /// </summary>
        /// <value>
        /// The error description.
        /// </value>
        public string ErrorDescription => Json.TryGetString(OidcConstants.TokenResponse.ErrorDescription);
    }
}
