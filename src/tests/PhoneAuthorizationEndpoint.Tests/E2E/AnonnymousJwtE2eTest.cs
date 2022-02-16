using TestBackend;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Diagnostics;

namespace AnonnymouAuthorizationEndpoint.Tests.E2E
{
    public class AnonnymousJwtE2eTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public AnonnymousJwtE2eTest(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetJwt()
        {
            var disco = await _client.GetDiscoveryDocumentAsync();
            var address = disco.Json.TryGetString("anonnymous_endpoint");
            using var pReq = new AnonnymousAuthorizationRequest
            {
                Address = address,
                ClientId = "mobile-app",
                Transport = "sms",
                Provider = "main-sms-provider",
                TransportData = "{\"phone\":\"+972542204119\"}",
                State = "some-app-state",
                Scope = "api1",
                RedirectUri = "https://localhost:5001/",
            };
            var anRes = await _client.RequestAnonnymousAuthorizationAsync(pReq);

            if (anRes.IsError)
                throw new Exception("Failed to authorize: " + anRes.ErrorDescription);

            Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", "https://localhost:5001/anonnymous?verification_code=" + anRes.VerificationCode);
            var res1 = await _client.GetAsync(disco.Issuer + "/anonnymous?verification_code=" + anRes.VerificationCode);

            var res2 = await _client.GetAsync(disco.Issuer + anRes.VerificationUriComplete);
            if (!res2.IsSuccessStatusCode)
                throw new Exception("Failed to activate");

            throw new NotImplementedException();
        }
    }
}
