using System;
using IdentityServer4.Events;
using IdentityServer4.Anonnymous.Validation;

namespace IdentityServer4.Anonnymous.Events
{
    public class AnonnymousAuthorizationFailureEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnonnymousAuthorizationFailureEvent"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public AnonnymousAuthorizationFailureEvent(AuthorizationRequestValidationResult result)
            : this()
        {
            if (result?.ValidatedRequest != null)
            {
                ClientId = result.ValidatedRequest.Client?.ClientId;
                ClientName = result.ValidatedRequest.Client?.ClientName;
                Scopes = result.ValidatedRequest.RequestedScopes?.ToDelimitedString(" ");
            }

            Endpoint = Constants.EndpointNames.AnonnymousAuthorization;
            Error = result?.Error;
            ErrorDescription = result?.ErrorDescription;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonnymousAuthorizationFailureEvent"/> class.
        /// </summary>
        public AnonnymousAuthorizationFailureEvent()
            : base(Constants.AnonnymousFlowEventCategory,
                "Anonnymous Authorization Failure",
                EventTypes.Failure,
                Constants.Events.AuthorizationFailureEventId)
        {
        }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the client.
        /// </summary>
        /// <value>
        /// The name of the client.
        /// </value>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the scopes.
        /// </summary>
        /// <value>
        /// The scopes.
        /// </value>
        public string Scopes { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the error description.
        /// </summary>
        /// <value>
        /// The error description.
        /// </value>
        public string ErrorDescription { get; set; }
    }
}