using IdentityModel;
using IdentityModel.Client;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace E2ETests
{
    class Program
    {
        private const string ServerAddress = "https://localhost:5001/";
        private static IDiscoveryCache Cache = new DiscoveryCache(ServerAddress);

        public static async Task Main()
        {
            Console.Title = "Console Anonymous Flow";

            await Task.Delay(10000);
            var authorizeResponse = await RequestAuthorizationAsync();
            var tokenResponse = await RequestTokenAsync(authorizeResponse);
            Console.WriteLine(tokenResponse.AccessToken);
            //tokenResponse.Show();

            Console.ReadLine();
            //await CallServiceAsync(tokenResponse.AccessToken);
        }
        static async Task<AnonymousAuthorizationResponse> RequestAuthorizationAsync()
        {
            var disco = await Cache.GetAsync();
            if (disco.IsError) throw new Exception(disco.Error);

            var address = disco.Json.TryGetString("anonymous_endpoint");
            using var pReq = new AnonymousAuthorizationRequest
            {
                Address = address,
                ClientId = "mobile-app",
                Transport = "sms",
                Provider = "main-sms-provider",
                TransportData = "{\"phone\":\"+972542204119\"}",
                State = "some-app-state",
                Scope = "api1",
                RedirectUri = ServerAddress,
            };

            var client = new HttpClient();
            var response = await client.RequestAnonymousAuthorizationAsync(pReq);

            if (response.IsError) throw new Exception(response.Error);

            //Console.WriteLine($"user code   : {response.}");
            Console.WriteLine($"verification code : {response.VerificationCode}");
            Console.WriteLine($"URL         : {response.VerificationUri}");
            Console.WriteLine($"Complete URL: {response.VerificationUriComplete}");

            Console.WriteLine($"\nPress enter to launch browser ({response.VerificationUri})");
            Console.ReadLine();

            var a = new Uri(new Uri(ServerAddress), response.VerificationUriComplete).ToString();
            Process.Start(new ProcessStartInfo(a) { UseShellExecute = true });
            return response;
        }

        private static async Task<TokenResponse> RequestTokenAsync(AnonymousAuthorizationResponse authorizeResponse)
        {
            var disco = await Cache.GetAsync();
            if (disco.IsError) throw new Exception(disco.Error);

            var client = new HttpClient();

            while (true)
            {
                var response = await client.RequestAnonymousTokenAsync(new AnonymousTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "mobile-app",
                    VerificationCode = authorizeResponse.VerificationCode
                });

                if (response.IsError)
                {
                    if (response.Error == OidcConstants.TokenErrors.AuthorizationPending || response.Error == OidcConstants.TokenErrors.SlowDown)
                    {
                        Console.WriteLine($"{response.Error}...waiting.");
                        await Task.Delay(authorizeResponse.Interval * 1000);
                    }
                    else
                    {
                        throw new Exception(response.Error);
                    }
                }
                else
                {
                    return response;
                }
            }
        }
        //static async Task CallServiceAsync(string token)
        //{
        //    var client = new HttpClient();

        //    client.SetBearerToken(token);
        //    var response = await client.GetStringAsync("http://localhost:3308/test");

        //    Console.WriteLine(JArray.Parse(response));
        //}
    }
}