using System;
using System.Linq;

namespace TwilioProviders
{
    public sealed class TwilioOptions
    {
        public const string Section = "TWILIO";
        public TwilioAccount[] Accounts { get; set; }
        public class TwilioAccount
        {
            public string AccountSid { get; set; }
            public string AuthToken { get; set; }
            public string Phone { get; set; }
            public string Provider { get; set; }
        }

        internal static bool Validate(TwilioOptions options)
        {
            var a = options.Accounts;
            if (a == default || a == Array.Empty<TwilioAccount>())
                throw new ArgumentException($"No Twilio accounts are configured");
            var len = a.Length;

            if (a.Where(x => x.AccountSid.HasValue()).Count() != len)
                throw new ArgumentException($"bad or missing config: {nameof(TwilioAccount.AccountSid)}");
            if (a.Where(x => x.AuthToken.HasValue()).Count() != len)
                throw new ArgumentException($"bad or missing config: {nameof(TwilioAccount.AuthToken)}");
            if (a.Where(x => x.Phone.HasValue()).Count() != len)
                throw new ArgumentException($"bad or missing config: {nameof(TwilioAccount.Phone)}");
            if (a.Where(x => x.Provider.HasValue()).Count() != len)
                throw new ArgumentException($"bad or missing config: {nameof(TwilioAccount.Provider)}");

            return true;
        }
    }
}