using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMQ
{
    public class RabbitMQConnectionFactory
    {
        private readonly IReadOnlyDictionary<string, RabbitMQPersistentConnection> _persistConnections;
        private readonly ILogger<RabbitMQConnectionFactory> _logger;

        public RabbitMQConnectionFactory(
            IEnumerable<RabbitMQPersistentConnection> connections,
            ILogger<RabbitMQConnectionFactory> logger)
        {
            _logger = logger;
            _persistConnections = connections.ToDictionary(
                k => k.Provider,
                v => v);
        }
        public (bool success, string error) TryPublish(string provider, string json)
        {
            _logger.LogDebug(nameof(TryPublish));
            if (!_persistConnections.TryGetValue(provider, out var con))
                return (false, $"failed to find rabbit-mq provider: \'{provider}\'");

            string error = default;
            var success = con.TryConnect() && con.TryPublish(json, out error);
            return (success, error);
        }
    }
}
