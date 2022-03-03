using IdentityServer4.Events;
using IdentityServer4.Anonymous.Validation;
using System;
using IdentityServer4.Anonymous.Services.Generators;

namespace IdentityServer4.Anonymous.Events
{
    public class AnonymousAuthorizationSuccessEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousAuthorizationSuccessEvent"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="request">The request.</param>
        public AnonymousAuthorizationSuccessEvent(
            AuthorizationResponse response, 
            AuthorizationRequestValidationResult request)
            : this()
        {
            Response = response;
            var vr = request.ValidatedRequest;
            ClientId = vr?.Client?.ClientId;
            ClientName = vr?.Client?.ClientName;
            Endpoint = Constants.EndpointNames.AnonymousAuthorization;
            Scopes = vr?.ValidatedResources?.RawScopeValues.ToDelimitedString(" ");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousAuthorizationSuccessEvent"/> class.
        /// </summary>
        protected AnonymousAuthorizationSuccessEvent()
            : base(Constants.AnonymousFlowEventCategory,
                "Anonymous Authorization Success",
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
        public AuthorizationResponse Response { get; }
    }
}