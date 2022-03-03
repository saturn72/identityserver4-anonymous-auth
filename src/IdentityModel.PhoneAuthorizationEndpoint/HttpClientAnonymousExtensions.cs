using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    public static class HttpClientAnonymousExtensions
    {
        /// <summary>
        /// Sends a userinfo request.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<AnonymousAuthorizationResponse> RequestAnonymousAuthorizationAsync(this HttpMessageInvoker client, AnonymousAuthorizationRequest request, CancellationToken cancellationToken = default)
        {
            var clone = request.Clone();

            clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.Scope, request.Scope);
            clone.Method = HttpMethod.Post;
            clone.Prepare();

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(clone, cancellationToken).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                return ProtocolResponse.FromException<AnonymousAuthorizationResponse>(ex);
            }

            var res = await ProtocolResponse.FromHttpResponseAsync<AnonymousAuthorizationResponse>(response).ConfigureAwait(true);

            return res;
        }
        public static async Task<TokenResponse> RequestAnonymousTokenAsync(this HttpMessageInvoker client, AnonymousTokenRequest request, CancellationToken cancellationToken = default)
        {
            var clone = request.Clone<AnonymousTokenRequest>();

            clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, Constants.GrantType);
            clone.Parameters.AddRequired(Constants.AnonymousAuthorizationResponse.VerificationCode, request.VerificationCode);

            return await client.RequestTokenAsync(clone, cancellationToken);
        }
    }
}
