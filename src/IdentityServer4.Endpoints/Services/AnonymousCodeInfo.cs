using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer4.Anonymous.Services
{
    public record AnonymousCodeInfo
    {
        public Guid Id { get; set; }
        public int AllowedRetries { get; set; }
        public IEnumerable<string> AuthorizedScopes { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
        public string ClientId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string Description { get; set; }
        public bool IsAuthorized { get; set; }
        public int Lifetime { get; set; }
        public IEnumerable<string> RequestedScopes { get; set; }
        public int RetryCounter { get; set; }
        public string ReturnUrl { get; set; }
        public string SessionId { get; set; }
        public string Subject { get; set; }
        public string Transport { get; set; }
        public string UserCode { get; set; }
        public DateTime? VerifiedOnUtc { get; set; }
        public string VerificationCode { get; set; }
    }
}
