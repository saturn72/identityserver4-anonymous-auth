using IdentityServer4.Models;
using IdentityServer4.PhoneAuthorizationEndpoint;
using System.Collections.Generic;

namespace Host
{
    public static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
            new ApiScope("api1", "My API")
            };
        public static IEnumerable<Client> Clients =>
    new List<Client>
    {
        new Client
        {
            ClientId = "mobile-app",

            // no interactive user, use the clientid/secret for authentication
            AllowedGrantTypes = {
                Constants.PhoneGrantType,
            },

            // secret for authentication
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },

            // scopes that client has access to
            AllowedScopes = { "api1" }
        }
    };
    }
}
