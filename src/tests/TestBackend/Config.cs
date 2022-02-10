using IdentityServer4.Models;
using IdentityServer4.Anonnymous;
using System.Collections.Generic;

namespace TestBackend
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
                Constants.AnonnymousGrantType,
            },
            RequireClientSecret = false,
            RedirectUris = { "https://localhost:5001/" },
            // scopes that client has access to
            AllowedScopes = { "api1" },
            Properties =
            {
                {"lifetime", "600" },
                {"transports", "[{\"name\":\"sms\", \"provider\":\"main-sms-provider\", \"config\":{\"key\":\"twilio\"}}]" },
                {"allowed_retries", "200" },
            },
        }
    };
    }
}
