using System.Linq;

namespace IdentityModel.Client
{
    public class AnonnymousAuthorizationRequest : ProtocolRequest
    {
        public string Scope { get; set; }
        public string Transport
        {
            get => Parameters.GetValues(Constants.AnonnymousAuthorizationRequest.Transport)?.FirstOrDefault();
            set => Parameters.Add(Constants.AnonnymousAuthorizationRequest.Transport, value);
        }
        public string Provider
        {
            get => Parameters.GetValues(Constants.AnonnymousAuthorizationRequest.Provider)?.FirstOrDefault();
            set => Parameters.Add(Constants.AnonnymousAuthorizationRequest.Provider, value);
        }
        public string TransportData
        {
            get => Parameters.GetValues(Constants.AnonnymousAuthorizationRequest.TransportData)?.FirstOrDefault();
            set => Parameters.Add(Constants.AnonnymousAuthorizationRequest.TransportData, value);
        }
        public string State
        {
            get => Parameters.GetValues(Constants.AnonnymousAuthorizationRequest.State)?.FirstOrDefault();
            set => Parameters.Add(Constants.AnonnymousAuthorizationRequest.State, value);
        }
        public string RedirectUri
        {
            get => Parameters.GetValues(Constants.AnonnymousAuthorizationRequest.RedirectUri)?.FirstOrDefault();
            set => Parameters.Add(Constants.AnonnymousAuthorizationRequest.RedirectUri, value);
        }
    }
}
