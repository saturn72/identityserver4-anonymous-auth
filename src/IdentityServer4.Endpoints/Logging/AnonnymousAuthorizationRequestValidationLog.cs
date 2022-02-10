using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Anonnymous.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace IdentityServer4.Anonnymous.Logging
{
    internal class AnonnymousAuthorizationRequestValidationLog

    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string Scopes { get; set; }

        public Dictionary<string, string> Raw { get; set; }

        private static readonly string[] SensitiveValuesFilter = {
            OidcConstants.TokenRequest.ClientSecret,
            OidcConstants.TokenRequest.ClientAssertion
        };

        public AnonnymousAuthorizationRequestValidationLog(ValidatedAnonnymousAuthorizationRequest request)
        {
            Raw = request.Raw.ToScrubbedDictionary(SensitiveValuesFilter);

            if (request.Client != null)
            {
                ClientId = request.Client.ClientId;
                ClientName = request.Client.ClientName;
            }

            if (request.RequestedScopes != null)
            {
                Scopes = request.RequestedScopes.ToDelimitedString(" ");
            }
        }

        public override string ToString()
        {
            return LogSerializer.Serialize(this);
        }
    }
}
