using IdentityServer4.Models;
using System.Security.Claims;

namespace IdentityServer4.Anonnymous.Services
{
    public record AnonnymousCodeValidationRequest
    {
        public AnonnymousCodeValidationRequest(
            AnonnymousCodeInfo code, 
            Client client, 
            ClaimsPrincipal subject)
        {
            Code = code;
            Client = client;
            Subject = subject;
        }
        public AnonnymousCodeInfo Code { get; }
        public Client Client { get; }
        public ClaimsPrincipal Subject { get; }
    }
}
