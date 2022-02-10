using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.Client
{
    public static class HttpClientAnonnymousFlowExtensions
    {
        /// <summary>
        /// Sends a userinfo request.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task<AnonnymousAuthorizationResponse> RequestAnonnymousAuthorizationAsync(this HttpMessageInvoker client, AnonnymousAuthorizationRequest request, CancellationToken cancellationToken = default)
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
                return ProtocolResponse.FromException<AnonnymousAuthorizationResponse>(ex);
            }

            var res = await ProtocolResponse.FromHttpResponseAsync<AnonnymousAuthorizationResponse>(response).ConfigureAwait(true);

            return res;
        }
    }
}
