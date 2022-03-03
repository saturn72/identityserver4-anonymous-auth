using System.Linq;

namespace IdentityModel.Client
{
    public class AnonymousAuthorizationRequest : ProtocolRequest
    {
        public string Scope { get; set; }
        public string Transport
        {
            get => Parameters.GetValues(Constants.AnonymousAuthorizationRequest.Transport)?.FirstOrDefault();
            set => Parameters.Add(Constants.AnonymousAuthorizationRequest.Transport, value);
        }
        public string Provider
        {
            get => Parameters.GetValues(Constants.AnonymousAuthorizationRequest.Provider)?.FirstOrDefault();
            set => Parameters.Add(Constants.AnonymousAuthorizationRequest.Provider, value);
        }
        public string TransportData
        {
            get => Parameters.GetValues(Constants.AnonymousAuthorizationRequest.TransportData)?.FirstOrDefault();
            set => Parameters.Add(Constants.AnonymousAuthorizationRequest.TransportData, value);
        }
        public string State
        {
            get => Parameters.GetValues(Constants.AnonymousAuthorizationRequest.State)?.FirstOrDefault();
            set => Parameters.Add(Constants.AnonymousAuthorizationRequest.State, value);
        }
        public string RedirectUri
        {
            get => Parameters.GetValues(Constants.AnonymousAuthorizationRequest.RedirectUri)?.FirstOrDefault();
            set => Parameters.Add(Constants.AnonymousAuthorizationRequest.RedirectUri, value);
        }
    }
}
