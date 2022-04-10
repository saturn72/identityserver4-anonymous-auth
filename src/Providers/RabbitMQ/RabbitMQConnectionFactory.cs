using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public class RabbitMQConnectionFactory
    {
        private readonly IReadOnlyDictionary<string, (RabbitMQOptions.RabbitMQConnectionInfo info, IRabbitMQPersistentConnection connection)> _persistConnections;
        private readonly ILogger<RabbitMQConnectionFactory> _logger;
        private readonly int _retryCount;

        public RabbitMQConnectionFactory(IServiceProvider services,
            ILogger<RabbitMQConnectionFactory> logger)
        {
            _logger = logger;
            var options = services.GetRequiredService<RabbitMQOptions>();
            _retryCount = options.RetryCounter;
            var log = services.GetService<ILoggerFactory>();
            _persistConnections = options.Connections.ToDictionary(
                k => k.Provider,
                v => CreateConnectionFactory(v, log.CreateLogger<DefaultRabbitMQPersistentConnection>()));
        }

        private static (RabbitMQOptions.RabbitMQConnectionInfo info, IRabbitMQPersistentConnection connection) CreateConnectionFactory(
            RabbitMQOptions.RabbitMQConnectionInfo info,
            ILogger<DefaultRabbitMQPersistentConnection> logger)
        {
            var cf = new ConnectionFactory
            {
                HostName = info.HostName,
                Port = info.Port,
                DispatchConsumersAsync = true,
            };
            cf.UserName = info.UsernameInternal;
            cf.Password = info.PasswordInternal;

            return (info, new DefaultRabbitMQPersistentConnection(cf, logger));
        }

        public async Task<(bool success, string error)> TryPublish(string provider, string json)
        {
            _logger.LogDebug(nameof(TryPublish));
            if (!_persistConnections.TryGetValue(provider, out var ci))
                return (false, $"failed to find rabbit-mq provider: \'{provider}\'");

            ci.connection.TryConnect();
            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetryAsync(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex.Message);
                });


            var body = Encoding.UTF8.GetBytes(json);
            await policy.ExecuteAsync(() =>
            {
                var properties = _publisherChannel.CreateBasicProperties();
                properties.Expiration = @event.Expiration.ToString();
                properties.DeliveryMode = 2; // persistent
                properties.MessageId = Guid.NewGuid();

                _logger.LogDebug("Publishing event to RabbitMQ: {EventId}", properties.MessageId);
                try
                {
                    _publisherChannel.BasicPublish(
                           exchange: ci.info.Exchange,
                           routingKey: ci.info.RoutingKey,
                           mandatory: true,
                           basicProperties: properties,
                           body: body);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                    return (false, ex.Message);
                }
            });

            return (true, default);
        }
    }
}
