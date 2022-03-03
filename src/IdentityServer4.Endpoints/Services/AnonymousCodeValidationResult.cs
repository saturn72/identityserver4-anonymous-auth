namespace IdentityServer4.Anonymous.Services
{
    public record AnonymousCodeValidationResult
    {
        public AnonymousCodeValidationRequest Request { get; }
        public bool IsError { get; set; }
        public string Message { get; set; }

        public AnonymousCodeValidationResult(AnonymousCodeValidationRequest request)
        {
            Request = request;
        }
    }
}
