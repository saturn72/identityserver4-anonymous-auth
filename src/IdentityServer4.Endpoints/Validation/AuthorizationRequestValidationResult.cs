using IdentityServer4.Anonnymous.Models;
using IdentityServer4.Validation;

namespace IdentityServer4.Anonnymous.Validation
{
    public class AuthorizationRequestValidationResult : ValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationRequestValidationResult"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        public AuthorizationRequestValidationResult(ValidatedAnonnymousAuthorizationRequest request)
        {
            IsError = false;
            ValidatedRequest = request;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationRequestValidationResult"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="error">The error.</param>
        /// <param name="errorDescription">The error description.</param>
        public AuthorizationRequestValidationResult(ValidatedAnonnymousAuthorizationRequest request, string error, string errorDescription = null)
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
        public ValidatedAnonnymousAuthorizationRequest ValidatedRequest { get; }
    }
}