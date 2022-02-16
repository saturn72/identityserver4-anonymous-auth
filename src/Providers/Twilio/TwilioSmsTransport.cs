using IdentityServer4.Anonnymous.Transport;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace TwilioProviders
{
    public class TwilioSmsTransport : ITransporter
    {
        private readonly TwilioOptions _options;
        private readonly ILogger<TwilioSmsTransport> _logger;

        public TwilioSmsTransport(
            IOptions<TwilioOptions> options,
            ILogger<TwilioSmsTransport> logger)
        {
            _options = options.Value;
            _logger = logger;
        }
        public virtual Func<UserCodeTransportContext, Task<bool>> ShouldHandle => ctx => Task.FromResult(ctx.Transport == "sms");
        public virtual Task Transport(UserCodeTransportContext context)
        {
            _logger.LogDebug(nameof(Transport));
            if (context == default) throw new ArgumentNullException(nameof(context));

            var account = _options.Accounts.FirstOrDefault(x => x.Provider == context.Provider);
            if (account == default)
            {
                _logger.LogError($"Cannot find twilio provider: \'{context.Provider}\'");
                return Task.CompletedTask;
            }
            if (!context.Data.TryParseAsJsonDocument(out var doc))
            {
                _logger.LogError($"Cannot parse provider-data: \'{context.Provider}\'");
                return Task.CompletedTask;
            }
            var phone = doc.RootElement.GetProperty("phone").GetString();
            if (!phone.HasValue() || !tryParsePhone(phone, out var toPhone))
            {
                _logger.LogError($"Failed to parse \'to\' phone number in provider's-data: {context.Data}\'");
                return Task.CompletedTask;
            }
            if (!tryParsePhone(account.Phone, out var fromPhone))
            {
                _logger.LogError($"Failed to parse account's phone. Account: {account}");
                return Task.CompletedTask;
            }

            TwilioClient.Init(account.AccountSid, account.AuthToken);
            _ = MessageResource.Create(
               body: context.Body,
               from: fromPhone,
               to: toPhone);
            return Task.CompletedTask;

            static bool tryParsePhone(string phone, out PhoneNumber value)
            {
                value = default;
                try
                {
                    value = new PhoneNumber(phone);
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }
    }
}
