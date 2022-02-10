using System;
using System.Collections.Generic;

namespace IdentityServer4.Anonnymous.Services
{
    public record AnonnymousCodeInfo
    {
        public Guid Id { get; set; }
        public DateTime ActivatedOnUtc { get; set; }
        public int AllowedRetries { get; set; }
        public string AnonnymousCode { get; set; }
        public IEnumerable<string> AuthorizedScopes { get; set; }
        public string ClientId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string Description { get; set; }
        public bool IsOpenId { get; set; }
        public int Lifetime { get; set; }
        public IEnumerable<string> RequestedScopes { get; set; }
        public int VerificationRetryCounter { get; set; }
        public string ReturnUrl { get; set; }
        public string Transport { get; set; }
        public string TransportData { get; set; }
        public string TransportProvider { get; set; }
        public DateTime? VerifiedOnUtc { get; set; }
        public string VerificationUri { get; set; }
    }
}
