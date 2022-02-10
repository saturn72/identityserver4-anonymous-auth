using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace IdentityServer4.Anonnymous.Endpoints
{
    public class AnonnymousVerificationEndpointHandler : IEndpointHandler
    {
        public Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}