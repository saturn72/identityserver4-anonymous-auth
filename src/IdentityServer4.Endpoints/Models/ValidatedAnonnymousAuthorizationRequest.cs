using IdentityServer4.Validation;
using System.Collections.Generic;

namespace IdentityServer4.Anonnymous.Models
{
    public class ValidatedAnonnymousAuthorizationRequest : ValidatedRequest
    {
        /// <summary>
        /// Gets or sets the requested scopes.
        /// </summary>
        /// <value>
        /// The scopes.
        /// </value>
        public IEnumerable<string> RequestedScopes { get; set; }

        /// <summary>
        /// Gets the description the user assigned to the device being authorized.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }
        public bool IsOpenIdRequest { get; internal set; }
        public string Provider { get; set; }
        public string Transport { get; set; }
        public string TransportData { get; set; }
        public string State { get; set; }
        public string RedirectUrl { get; set; }
        public string VerificationUri { get; set; }
    }
}