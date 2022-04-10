using IdentityServer4.Anonymous.Endpoints.Results;
using Shouldly;
using System;
using Xunit;

namespace Identityserver4.Anonymous.Tests.Endpoints.Results
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
