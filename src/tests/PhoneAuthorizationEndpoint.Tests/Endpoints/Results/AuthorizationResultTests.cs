using IdentityServer4.Anonnymous.Endpoints.Results;
using IdentityServer4.Anonnymous.ResponseHandling;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PhoneAuthorizationEndpoint.Tests.Endpoints.Results
{
    public class AuthorizationResultTests
    {
        [Fact]
        public void ThrowsOnCtoNullParameter()
        {
            Should.Throw<ArgumentNullException>(() => new AuthorizationResult(null));
        }
    }
}
