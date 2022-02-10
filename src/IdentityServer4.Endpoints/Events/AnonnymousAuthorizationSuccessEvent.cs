using IdentityServer4.Events;
using IdentityServer4.Anonnymous.Validation;
using System;
using IdentityServer4.Anonnymous.ResponseHandling;

namespace IdentityServer4.Anonnymous.Events
{
    public class AnonnymousAuthorizationSuccessEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnonnymousAuthorizationSuccessEvent"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="request">The request.</param>
        public AnonnymousAuthorizationSuccessEvent(AuthorizationResponse response, AuthorizationRequestValidationResult request)
            : this()
        {
            ClientId = request.ValidatedRequest.Client?.ClientId;
            ClientName = request.ValidatedRequest.Client?.ClientName;
            Endpoint = Constants.EndpointNames.AnonnymousAuthorization;
            Scopes = request.ValidatedRequest.ValidatedResources?.RawScopeValues.ToDelimitedString(" ");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonnymousAuthorizationSuccessEvent"/> class.
        /// </summary>
        protected AnonnymousAuthorizationSuccessEvent()
            : base(Constants.AnonnymousFlowEventCategory,
                "Anonnymous Authorization Success",
                EventTypes.Success,
                Constants.Events.AuthorizationSuccessEventId)
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
    }
}