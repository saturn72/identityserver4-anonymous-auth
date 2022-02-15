using IdentityServer4.Anonnymous.Endpoints.Results;
using IdentityServer4.ResponseHandling;
using Shouldly;
using System;
using Xunit;

namespace PhoneAuthorizationEndpoint.Tests.Endpoints.Results
{
    public class TokenErrorResultTests
    {
        [Fact]
        public void ThrowsOnCtorNullParameter_1()
        {
            Should.Throw<ArgumentNullException>(() => new TokenErrorResult(null));
        }
        [Fact]
        public void ThrowsOnCtorNullParameter_2()
        {
            Should.Throw<ArgumentNullException>(() => new TokenErrorResult(new TokenErrorResponse { Error = null }));
        }
    }
}
