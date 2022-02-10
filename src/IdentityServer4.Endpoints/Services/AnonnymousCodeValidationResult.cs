namespace IdentityServer4.Anonnymous.Services
{
    public record AnonnymousCodeValidationResult
    {
        public AnonnymousCodeValidationRequest Request { get; }
        public bool IsError { get; set; }
        public string Message { get; set; }

        public AnonnymousCodeValidationResult(AnonnymousCodeValidationRequest request)
        {
            Request = request;
        }
    }
}
