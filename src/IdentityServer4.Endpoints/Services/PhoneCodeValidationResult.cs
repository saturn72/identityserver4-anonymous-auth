namespace IdentityServer4.PhoneAuthorizationEndpoint.Services
{
    public record PhoneCodeValidationResult
    {
        public PhoneCodeValidationRequest Request { get; }
        public bool IsError { get; set; }
        public string Message { get; set; }

        public PhoneCodeValidationResult(PhoneCodeValidationRequest request)
        {
            Request = request;
        }
    }
}
