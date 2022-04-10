using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace IdentityServer4.Anonymous.Tests
{
    public class AnonymousAuthorizationOptionsTests
    {
        [Fact]
        public void Section()
        {
            AnonymousAuthorizationOptions.Section.ShouldBe("anonymous");
        }
        [Fact]
        public void AllProperties()
        {
            var o = new AnonymousAuthorizationOptions();
            o.AllowedRetries.ShouldBe(5);
            o.AllowedRetriesPropertyName.ShouldBe(Constants.ClientProperties.AllowedRetries);
            o.InputLengthRestrictions.ShouldNotBeNull();
            o.DefaultLifetime.ShouldBe(60);
            o.DefaultUserCodeType.ShouldBe("5_figures_user_code");
            o.DefaultUserCodeSmsFormat.ShouldBe(Constants.Formats.Messages.UserCodeSmsFormat);
            o.DefaultUserCodeEmailFormat.ShouldBe(Constants.Formats.Messages.UserCodeEmailFormat);
            o.DefaultUserCodeEmailFormatPropertyName.ShouldBe(Constants.ClientProperties.UserCodeEmailFormat);
            o.DefaultUserCodeSmSFormatPropertyName.ShouldBe(Constants.ClientProperties.UserCodeSmsFormat);
            o.Interval.ShouldBe(5);
            o.LifetimePropertyName.ShouldBe(Constants.ClientProperties.Lifetime);
            o.Transports.ShouldBeEmpty();
            o.VerificationUri.ShouldBe(Constants.EndpointPaths.VerificationUri);
        }

        [Theory]
        [MemberData(nameof(AnonymousAuthorizationOptions_Validate_DATA))]
        public void AnonymousAuthorizationOptions_Validate(AnonymousAuthorizationOptions options, string msg)
        {
            var e = Should.Throw<ArgumentException>(() => AnonymousAuthorizationOptions.Validate(options));
            e.Message.ShouldContain(msg);
        }
        public static IEnumerable<object[]> AnonymousAuthorizationOptions_Validate_DATA = new[]
        {
            new object[] { new AnonymousAuthorizationOptions{ AllowedRetries = default },  $"bad or missing config: {nameof(AnonymousAuthorizationOptions.AllowedRetries)}" },
            new object[] { new AnonymousAuthorizationOptions{ AllowedRetriesPropertyName = default },  $"bad or missing config: {nameof(AnonymousAuthorizationOptions.AllowedRetriesPropertyName)}" },
            new object[] { new AnonymousAuthorizationOptions{ AllowedRetriesPropertyName = "" },  $"bad or missing config: {nameof(AnonymousAuthorizationOptions.AllowedRetriesPropertyName)}" },
            new object[] { new AnonymousAuthorizationOptions{ AllowedRetriesPropertyName = " " },  $"bad or missing config: {nameof(AnonymousAuthorizationOptions.AllowedRetriesPropertyName)}" },
            new object[] { new AnonymousAuthorizationOptions{ DefaultLifetime = default },  $"bad or missing config: {nameof(AnonymousAuthorizationOptions.DefaultLifetime)}" },
            new object[] { new AnonymousAuthorizationOptions{ DefaultUserCodeType = default },  $"bad or missing config: {nameof(AnonymousAuthorizationOptions.DefaultUserCodeType)}" },
            new object[] { new AnonymousAuthorizationOptions{ DefaultUserCodeType = "" },  $"bad or missing config: {nameof(AnonymousAuthorizationOptions.DefaultUserCodeType)}" },
            new object[] { new AnonymousAuthorizationOptions{ DefaultUserCodeType = " " },  $"bad or missing config: {nameof(AnonymousAuthorizationOptions.DefaultUserCodeType)}" },
            new object[] { new AnonymousAuthorizationOptions{ InputLengthRestrictions = default },  $"bad or missing config: {nameof(AnonymousAuthorizationOptions.InputLengthRestrictions)}" },
            new object[] { new AnonymousAuthorizationOptions { Interval = default },  $"bad or missing config: {nameof(AnonymousAuthorizationOptions.Interval)}" },
            new object[] { new AnonymousAuthorizationOptions{ LifetimePropertyName = default },  $"bad or missing config: {nameof(AnonymousAuthorizationOptions.LifetimePropertyName)}" },
            new object[] { new AnonymousAuthorizationOptions{ LifetimePropertyName = "" },  $"bad or missing config: {nameof(AnonymousAuthorizationOptions.LifetimePropertyName)}" },
            new object[] { new AnonymousAuthorizationOptions{ LifetimePropertyName = " " },  $"bad or missing config: {nameof(AnonymousAuthorizationOptions.LifetimePropertyName)}" },
        };
        [Fact]
        public void AnonymousAuthorizationOptions_Validates()
        {
            AnonymousAuthorizationOptions.Validate(new AnonymousAuthorizationOptions()).ShouldBeTrue();
        }
    }
}
