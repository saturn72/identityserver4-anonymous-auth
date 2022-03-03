using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace IdentityServer4.Anonymous.Endpoints.Results
{
    public class TokenErrorResult : IEndpointResult
    {
        public TokenErrorResponse Response { get; }

        public TokenErrorResult(TokenErrorResponse error)
        {
            if (error == default || !error.Error.HasValue())
                throw new ArgumentNullException(nameof(error.Error), "Error must be set");

            Response = error;
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            context.Response.StatusCode = 400;
            context.Response.SetNoCache();

            var dto = new
            {
                error = Response.Error,
                error_description = Response.ErrorDescription,
            };

            await context.Response.WriteJsonAsync(dto);
        }
    }
}
