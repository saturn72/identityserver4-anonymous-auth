using System;
using System.Linq;

namespace RabbitMQ
{
    public class RabbitMQOptions
    {
        public const string Section = "RABBITMQ";
        public RabbitMQConnectionInfo[] Connections { get; set; }
        public int RetryCounter { get; set; } = 5;
        public class RabbitMQConnectionInfo
        {
            private string _password;
            private string _passwordInternal;
            private string _username;
            private string _usernameInternal;

            public string Exchange { get; set; }
            public string HostName { get; set; }
            public string Password
            {
                get => _password;
                set
                {
                    _password = value;
                    _passwordInternal = _password + ":password";
                }
            }
            public string PasswordInternal => _passwordInternal;
            public int Port { get; set; }
            public string Provider { get; set; }
            public string RoutingKey { get; set; }
            public string Type { get; set; }
            public string Username
            {
                get => _username;
                set
                {
                    _username = value;
                    _usernameInternal = _username + ":username";
                }
            }
            public string UsernameInternal => _usernameInternal;
        }
        internal static bool Validate(RabbitMQOptions options)
        {
            var c = options.Connections;
            if (c == default || c == Array.Empty<RabbitMQConnectionInfo>())
                throw new ArgumentException($"No Twilio accounts are configured");
            var len = c.Length;

            if (c.Where(x => x.Exchange.HasValue()).Count() != len)
                throw new ArgumentException($"bad or missing config: {nameof(RabbitMQConnectionInfo.Exchange)}");

            if (c.Where(x => x.HostName.HasValue()).Count() != len)
                throw new ArgumentException($"bad or missing config: {nameof(RabbitMQConnectionInfo.HostName)}");

            if (c.Where(x => x.Port <= 0).Any())
                throw new ArgumentException($"bad or missing config: {nameof(RabbitMQConnectionInfo.Port)}");

            if (c.Where(x => x.Password.HasValue()).Count() != len)
                throw new ArgumentException($"bad or missing config: {nameof(RabbitMQConnectionInfo.Password)}");

            if (options.Connections.GroupBy(a => a.Provider).Count() != len)
                throw new ArgumentException($"bad or missing config: {nameof(RabbitMQConnectionInfo.Provider)} (non-unique provider name)");

            if (c.Where(x => x.Provider.HasValue()).Count() != len)
                throw new ArgumentException($"bad or missing config: {nameof(RabbitMQConnectionInfo.Provider)}");

            if (c.Where(x => x.Type.HasValue()).Count() != len)
                throw new ArgumentException($"bad or missing config: {nameof(RabbitMQConnectionInfo.Type)}");

            if (c.Where(x => x.Username.HasValue()).Count() != len)
                throw new ArgumentException($"bad or missing config: {nameof(RabbitMQConnectionInfo.Username)}");

            if (c.Where(x => x.RoutingKey.HasValue()).Count() != len)
                throw new ArgumentException($"bad or missing config: {nameof(RabbitMQConnectionInfo.RoutingKey)}");

            return true;
        }
    }
}
