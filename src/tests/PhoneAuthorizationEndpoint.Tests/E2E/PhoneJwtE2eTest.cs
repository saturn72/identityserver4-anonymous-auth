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
        private const string BaseAddress = "http://localhost:5000";

        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public PhoneJwtE2eTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Test1()
        {
            var disco = await _client.GetDiscoveryDocumentAsync();

            throw new NotImplementedException();
        }
    }
}
