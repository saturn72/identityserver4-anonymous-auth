using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using System;
using IdentityServer4.Anonnymous.ResponseHandling;

namespace IdentityServer4.Anonnymous.Endpoints.Results
{
    internal class AuthorizationResult : IEndpointResult
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
                anonnymous_code = Response.AnonnymousCode,
                user_code = Response.UserCode,
                activation_uri = Response.ActivationUri,
                activation_uri_complete = Response.ActivationUriComplete,
                expires_in = Response.AnonnymousCodeLifetime,
                interval = Response.Interval
            };

            await context.Response.WriteJsonAsync(dto);
        }
    }
}