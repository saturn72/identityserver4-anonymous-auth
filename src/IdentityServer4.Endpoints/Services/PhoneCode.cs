using System;
using System.Collections.Generic;

namespace IdentityServer4.PhoneAuthorizationEndpoint.Services
{
    public record PhoneCode
    {
        public Guid Id { get; set; }
        public int AllowedRetries { get; set; }
        public bool AllowUserCodeTransfer { get; set; }
        public IEnumerable<string> AuthorizedScopes { get; set; }
        public string ClientId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string Description { get; set; }
        public bool IsAuthorized { get; set; }
        public bool IsOpenId { get; set; }
        public int Lifetime { get; set; }
        public string RequestIdentityProvider { get; set; }
        public IEnumerable<string> RequestedScopes { get; set; }
        public int RetriesCounter { get; set; }
        public string ReturnUrl { get; set; }
        public string RequestSubjectId { get; set; }
        public string SessionId { get; set; }
        public string Transport { get; set; }
        public string VerificationIdentityProvider { get; set; }
        public string VerificationSubjectId { get; set; }
        public DateTime? VerifiedOnUtc { get; set; }
    }
}
