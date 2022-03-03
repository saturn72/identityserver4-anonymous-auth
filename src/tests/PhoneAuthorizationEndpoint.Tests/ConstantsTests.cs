using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.Anonymous.Tests
{
    public class ConstantsTests
    {
        [Fact]
        public void Constants_Test()
        {
            Constants.AnonymousFlowEventCategory.ShouldBe("Anonymous");
            Constants.AnonymousGrantType.ShouldBe("anonymous");
            Constants.AnonymousAuthenticationType.ShouldBe("anonymous");
        }
        [Fact]
        public void FormParameters_Tests()
        {
            Constants.FormParameters.Transport.ShouldBe("transport");
            Constants.FormParameters.Provider.ShouldBe("provider");
            Constants.FormParameters.TransportData.ShouldBe("transport_data");
            Constants.FormParameters.State.ShouldBe("state");
            Constants.FormParameters.RedirectUri.ShouldBe("redirect_uri");
        }
        [Fact]
        public void ClientProperties_Tests()
        {
            Constants.ClientProperties.Transports.ShouldBe("transports");
            Constants.ClientProperties.TransportName.ShouldBe("name");
            Constants.ClientProperties.TransportProvider.ShouldBe("provider");
            Constants.ClientProperties.AllowedRetries.ShouldBe("allowed_retries");
            Constants.ClientProperties.Lifetime.ShouldBe("lifetime");
            Constants.ClientProperties.UserCodeEmailFormat.ShouldBe("formats:user_code");
            Constants.ClientProperties.UserCodeSmsFormat.ShouldBe("formats:user_code");
        }

        [Fact]
        public void Formats_Tests()
        {
            Constants.Formats.Fields.UserCode.ShouldBe("{{USER_CODE}}");
            Constants.Formats.Messages.UserCodeSmsFormat.ShouldBe("User code is: {{USER_CODE}}");
            Constants.Formats.Messages.UserCodeEmailFormat.ShouldBe("User code is: {{USER_CODE}}");
        }

        [Fact]
        public void EndpointNames_Tests()
        {
            Constants.EndpointNames.AnonymousAuthorization.ShouldBe("anonymous_endpoint");
        }
        [Fact]
        public void EndpointPaths_Tests()
        {
            Constants.EndpointPaths.AuthorizationEndpoint.ShouldBe("/connect/anonymous");
            Constants.EndpointPaths.VerificationUri.ShouldBe("/anonymous/verify");
        }

        [Fact]
        public void Events_Tests()
        {
            Constants.Events.AuthorizationSuccessEventId.ShouldBe(10000);
            Constants.Events.AuthorizationFailureEventId.ShouldBe(10001);
            Constants.Events.GrantSuccessEventName.ShouldBe("anonymous-grant-success");
            Constants.Events.GrantSuccessEventId.ShouldBe(10010);
            Constants.Events.GrantFailedEventName.ShouldBe("anonymous-grant-failed");
            Constants.Events.GrantFailedEventId.ShouldBe(10011);
        }
        
        [Fact]
        public void UserInteraction_Tests()
        {
            Constants.UserInteraction.VerificationCode.ShouldBe("verification_code");
            Constants.UserInteraction.UserCode.ShouldBe("user_code");
        }
     
        [Fact]
        public void TransportTypes_Tests()
        {
            Constants.TransportTypes.Sms.ShouldBe("sms");
            Constants.TransportTypes.Email.ShouldBe("email");
        }
    }
}
