using IdentityServer4.Events;
using IdentityServer4.PhoneAuthorizationEndpoint.Validation;
using System;

namespace IdentityServer4.PhoneAuthorizationEndpoint.Events
{
    public class PhoneAuthorizationSuccessEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneAuthorizationSuccessEvent"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="request">The request.</param>
        public PhoneAuthorizationSuccessEvent(PhoneAuthorizationResponse response, PhoneAuthorizationRequestValidationResult request)
            : this()
        {
            ClientId = request.ValidatedRequest.Client?.ClientId;
            ClientName = request.ValidatedRequest.Client?.ClientName;
            Endpoint = Constants.EndpointNames.PhoneAuthorization;
            Scopes = request.ValidatedRequest.ValidatedResources?.RawScopeValues.ToDelimitedString(" ");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneAuthorizationSuccessEvent"/> class.
        /// </summary>
        protected PhoneAuthorizationSuccessEvent()
            : base(Constants.PhoneFlowEventCategory,
                "Phone Authorization Success",
                EventTypes.Success,
                Constants.Events.PhoneAuthorizationSuccessEventId)
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