using System.Linq;

namespace IdentityModel.Client
{
    public class PhoneAuthorizationRequest : ProtocolRequest
    {
        public string Scope { get; set; }
        public string Transport
        {
            get => Parameters.GetValues(Constants.PhoneAuthorizationRequest.Transport)?.FirstOrDefault();
            set => Parameters.Add(Constants.PhoneAuthorizationRequest.Transport, value);
        }
        public string TransportData
        {
            get => Parameters.GetValues(Constants.PhoneAuthorizationRequest.TransportData)?.FirstOrDefault();
            set => Parameters.Add(Constants.PhoneAuthorizationRequest.TransportData, value);
        }
        public string State
        {
            get => Parameters.GetValues(Constants.PhoneAuthorizationRequest.State)?.FirstOrDefault();
            set => Parameters.Add(Constants.PhoneAuthorizationRequest.State, value);
        }
    }
}
