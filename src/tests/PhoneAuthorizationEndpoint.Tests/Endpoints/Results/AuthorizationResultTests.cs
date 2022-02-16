using IdentityServer4.Anonnymous.Endpoints.Results;
using Shouldly;
using System;
using Xunit;

namespace Identityserver4.Anonnymous.Tests.Endpoints.Results
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
