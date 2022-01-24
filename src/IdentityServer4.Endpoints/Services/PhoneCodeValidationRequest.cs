using IdentityServer4.Models;
using System.Security.Claims;

namespace IdentityServer4.PhoneAuthorizationEndpoint.Services
{
    public record PhoneCodeValidationRequest
    {
        public PhoneCodeValidationRequest(PhoneCode phoneCode, Client client, ClaimsPrincipal subject)
        {
            PhoneCode = phoneCode;
            Client = client;
            Subject = subject;
        }
        public PhoneCode PhoneCode { get; }
        public Client Client { get; }
        public ClaimsPrincipal Subject { get; }
    }
}
