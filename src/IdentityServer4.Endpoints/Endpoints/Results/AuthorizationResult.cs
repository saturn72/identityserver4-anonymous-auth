using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using System;
using IdentityServer4.Anonnymous.ResponseHandling;

namespace IdentityServer4.Anonnymous.Endpoints.Results
{
    public class AuthorizationResult : IEndpointResult
    {
        public AuthorizationResponse Response { get; }

        public AuthorizationResult(AuthorizationResponse response)
        {
            Response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            context.Response.SetNoCache();

            var dto = new
            {
                verification_code = Response.VerificationCode,
                verification_uri = Response.VerificationUri,
                verification_uri_complete = Response.VerificationUriComplete,
                expires_in = Response.Lifetime,
                interval = Response.Interval
            };

            await context.Response.WriteJsonAsync(dto);
        }
    }
}