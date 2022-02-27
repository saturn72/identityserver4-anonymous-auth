using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace IdentityServer4.Anonnymous.Tests
{
    public class AnonnymousAuthorizationOptionsTests
    {
        [Fact]
        public void Section()
        {
            AnonnymousAuthorizationOptions.Section.ShouldBe("anonnymous");
        }
        [Fact]
        public void AllProperties()
        {
            var o = new AnonnymousAuthorizationOptions();
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
        [MemberData(nameof(AnonnymousAuthorizationOptions_Validate_DATA))]
        public void AnonnymousAuthorizationOptions_Validate(AnonnymousAuthorizationOptions options, string msg)
        {
            var e = Should.Throw<ArgumentException>(() => AnonnymousAuthorizationOptions.Validate(options));
            e.Message.ShouldContain(msg);
        }
        public static IEnumerable<object[]> AnonnymousAuthorizationOptions_Validate_DATA = new[]
        {
            new object[] { new AnonnymousAuthorizationOptions{ AllowedRetries = default },  $"bad or missing config: {nameof(AnonnymousAuthorizationOptions.AllowedRetries)}" },
            new object[] { new AnonnymousAuthorizationOptions{ AllowedRetriesPropertyName = default },  $"bad or missing config: {nameof(AnonnymousAuthorizationOptions.AllowedRetriesPropertyName)}" },
            new object[] { new AnonnymousAuthorizationOptions{ AllowedRetriesPropertyName = "" },  $"bad or missing config: {nameof(AnonnymousAuthorizationOptions.AllowedRetriesPropertyName)}" },
            new object[] { new AnonnymousAuthorizationOptions{ AllowedRetriesPropertyName = " " },  $"bad or missing config: {nameof(AnonnymousAuthorizationOptions.AllowedRetriesPropertyName)}" },
            new object[] { new AnonnymousAuthorizationOptions{ DefaultLifetime = default },  $"bad or missing config: {nameof(AnonnymousAuthorizationOptions.DefaultLifetime)}" },
            new object[] { new AnonnymousAuthorizationOptions{ DefaultUserCodeType = default },  $"bad or missing config: {nameof(AnonnymousAuthorizationOptions.DefaultUserCodeType)}" },
            new object[] { new AnonnymousAuthorizationOptions{ DefaultUserCodeType = "" },  $"bad or missing config: {nameof(AnonnymousAuthorizationOptions.DefaultUserCodeType)}" },
            new object[] { new AnonnymousAuthorizationOptions{ DefaultUserCodeType = " " },  $"bad or missing config: {nameof(AnonnymousAuthorizationOptions.DefaultUserCodeType)}" },
            new object[] { new AnonnymousAuthorizationOptions{ InputLengthRestrictions = default },  $"bad or missing config: {nameof(AnonnymousAuthorizationOptions.InputLengthRestrictions)}" },
            new object[] { new AnonnymousAuthorizationOptions { Interval = default },  $"bad or missing config: {nameof(AnonnymousAuthorizationOptions.Interval)}" },
            new object[] { new AnonnymousAuthorizationOptions{ LifetimePropertyName = default },  $"bad or missing config: {nameof(AnonnymousAuthorizationOptions.LifetimePropertyName)}" },
            new object[] { new AnonnymousAuthorizationOptions{ LifetimePropertyName = "" },  $"bad or missing config: {nameof(AnonnymousAuthorizationOptions.LifetimePropertyName)}" },
            new object[] { new AnonnymousAuthorizationOptions{ LifetimePropertyName = " " },  $"bad or missing config: {nameof(AnonnymousAuthorizationOptions.LifetimePropertyName)}" },
        };
    }
}
