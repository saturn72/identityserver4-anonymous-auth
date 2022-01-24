using System.Threading.Tasks;

namespace IdentityServer4.PhoneAuthorizationEndpoint.Services
{
    interface IPhoneCodeTransport
    {
        /// <summary>
        /// Unique identifier for the transport
        /// </summary>
        string Type { get; }

        Task Transport(PhoneCodeTransportContext context);
    }
}
