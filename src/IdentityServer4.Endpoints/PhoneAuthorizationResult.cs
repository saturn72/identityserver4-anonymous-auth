using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using System;

namespace IdentityServer4.PhoneAuthorizationEndpoint
{
    internal class PhoneAuthorizationResult : IEndpointResult
    {
        public PhoneAuthorizationResponse Response { get; }

        public PhoneAuthorizationResult(PhoneAuthorizationResponse response)
        {
            Response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            context.Response.SetNoCache();

            var dto = new ResultDto
            {
                phone_code = Response.PhoneCode,
                user_code = Response.UserCode,
                verification_uri = Response.VerificationUri,
                verification_uri_complete = Response.VerificationUriComplete,
                expires_in = Response.PhoneCodeLifetime,
                interval = Response.Interval
            };

            await context.Response.WriteJsonAsync(dto);
        }

        internal class ResultDto
        {
            public string phone_code { get; set; }
            public string user_code { get; set; }
            public string verification_uri { get; set; }
            public string verification_uri_complete { get; set; }
            public int expires_in { get; set; }
            public int interval { get; set; }
        }
    }
}