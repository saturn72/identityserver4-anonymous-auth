using IdentityServer4.Models;
using System.Security.Claims;

namespace IdentityServer4.Anonymous.Services
{
    public record AnonymousCodeValidationRequest
    {
        public AnonymousCodeValidationRequest(
            AnonymousCodeInfo code, 
            Client client, 
            ClaimsPrincipal subject)
        {
            Code = code;
            Client = client;
            Subject = subject;
        }
        public AnonymousCodeInfo Code { get; }
        public Client Client { get; }
        public ClaimsPrincipal Subject { get; }
    }
}
