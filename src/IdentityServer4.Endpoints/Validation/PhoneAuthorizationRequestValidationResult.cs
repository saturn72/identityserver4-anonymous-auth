using IdentityServer4.PhoneAuthorizationEndpoint.Models;
using IdentityServer4.Validation;

namespace IdentityServer4.PhoneAuthorizationEndpoint.Validation
{
    public class PhoneAuthorizationRequestValidationResult : ValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneAuthorizationRequestValidationResult"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        public PhoneAuthorizationRequestValidationResult(ValidatedPhoneAuthorizationRequest request)
        {
            IsError = false;

            ValidatedRequest = request;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneAuthorizationRequestValidationResult"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="error">The error.</param>
        /// <param name="errorDescription">The error description.</param>
        public PhoneAuthorizationRequestValidationResult(ValidatedPhoneAuthorizationRequest request, string error, string errorDescription = null)
        {
            IsError = true;

            Error = error;
            ErrorDescription = errorDescription;
            ValidatedRequest = request;
        }

        /// <summary>
        /// Gets the validated request.
        /// </summary>
        /// <value>
        /// The validated request.
        /// </value>
        public ValidatedPhoneAuthorizationRequest ValidatedRequest { get; }
    }
}