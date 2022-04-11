using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace RabbitMQ
{
    public class RabbitMQPersistentConnection
    {
        private readonly RabbitMQOptions.RabbitMQConnectionInfo _info;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMQPersistentConnection> _logger;

        internal object Select(Action<object> p)
        {
            throw new NotImplementedException();
        }

        private IConnection _connection;
        bool _disposed;
        private readonly object sync_root = new();

        public string Provider { get; }
        public RabbitMQPersistentConnection(
            RabbitMQOptions.RabbitMQConnectionInfo info,
            ILogger<RabbitMQPersistentConnection> logger)
        {
            _info = info;
            _connectionFactory = new ConnectionFactory
            {
                HostName = info.Host,
                Port = info.Port,
                DispatchConsumersAsync = true,
            };
            _connectionFactory.UserName = info.UsernameInternal;
            _connectionFactory.Password = info.PasswordInternal;
            Provider = _info.Provider;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            lock (sync_root)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_info.RetryCounter, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                    }
                );

                policy.Execute(() =>
                {
                    _connection = _info.Endpoints.IsNullOrEmpty() ?
                        _connectionFactory.CreateConnection() :
                        _connectionFactory.CreateConnection(_info.Endpoints);
                });

                if (_connection != null && _connection.IsOpen && !_disposed)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;

                    _logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _connection.Endpoint.HostName);

                    return true;
                }
                else
                {
                    _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");
                    return false;
                }
            }
        }

        internal bool TryPublish(string message, out string error)
        {
            string err = null;
            var policy = Policy.Handle<BrokerUnreachableException>()

               .Or<SocketException>()
               .WaitAndRetry(_info.RetryCounter, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
               {
                   _logger.LogWarning(ex.Message);
               });


            var body = Encoding.UTF8.GetBytes(message);
            throw new System.NotImplementedException();
            //policy.Execute(() =>
            //{
            //    var properties = _connection.CreateBasicProperties();

            //    properties.Expiration = @event.Expiration.ToString;

            //    properties.DeliveryMode = 2; // persistent
            //    properties.MessageId = Guid.NewGuid();

            //    _logger.LogDebug("Publishing event to RabbitMQ: {EventId}", properties.MessageId);
            //    try
            //    {
            //        _connection.BasicPublish(
            //               exchange: _info.Exchange,
            //               routingKey: _info.RoutingKey,
            //               mandatory: true,
            //               basicProperties: properties,
            //               body: body);
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex.Message, ex);
            //        err = ex.Message;
            //        return false;
            //    }
            //});
            error = err;
            return true;
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

            TryConnect();
        }

        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }

        void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
        }
    }
}
