using Host;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PhoneAuthorizationEndpoint.Tests.E2E
{
    public class PhoneJwtE2eTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public PhoneJwtE2eTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetJwt()
        {
            var disco = await _client.GetDiscoveryDocumentAsync();
            var address = disco.Json.TryGetString("phone_endpoint");
            using var pReq = new PhoneAuthorizationRequest
            {
                Address = address,
                ClientId = "mobile-app",
                Transport ="fcm",
                TransportData = "fcm-con-id",
                State = "state",                
            };
            var res = await _client.RequestPhoneAuthorizationAsync(pReq);
            
            throw new NotImplementedException();
        }
    }
}
