using IdentityServer4.Anonymous.Transport;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public class RabbitMQTransport : ITransporter
    {
        private readonly RabbitMQConnectionFactory _factory;
        private readonly RabbitMQOptions _options;
        private readonly ILogger<RabbitMQTransport> _logger;

        public virtual Func<UserCodeTransportContext, Task<bool>> ShouldHandle => ctx => Task.FromResult(ctx.Transport == "rabbitmq");
        public RabbitMQTransport(
            RabbitMQConnectionFactory factory,
            IOptions<RabbitMQOptions> options,
            ILogger<RabbitMQTransport> logger)
        {
            _factory = factory;
            _options = options.Value;
            _logger = logger;
        }

        public async Task Transport(UserCodeTransportContext context)
        {
            _logger.LogDebug(nameof(Transport));
            if (context == default) throw new ArgumentNullException(nameof(context));

            //parse the payload here
            if (!context.Data.TryParseAsJsonDocument(out var doc))
            {
                _logger.LogError($"Cannot parse provider-data: \'{context.Provider}\'");
                return;
            }
            var json = "json stroing";

            var (success, error) = await _factory.TryPublish(context.Provider, json);
            if (!success)
                _logger.LogError(error);
        }
    }
}
