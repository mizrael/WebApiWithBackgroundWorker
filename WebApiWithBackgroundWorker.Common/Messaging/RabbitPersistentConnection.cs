using System;
using RabbitMQ.Client;

namespace WebApiWithBackgroundWorker.Common.Messaging
{

    public class RabbitPersistentConnection : IDisposable, IBusConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private bool _disposed;

        private readonly object semaphore = new object();

        public RabbitPersistentConnection(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _connection.Dispose();
        }

        private void TryConnect()
        {
            lock (semaphore)
            {
                if (IsConnected)
                    return;

                _connection = _connectionFactory.CreateConnection();
                _connection.ConnectionShutdown += (s, e) => TryConnect();
                _connection.CallbackException += (s, e) => TryConnect();
                _connection.ConnectionBlocked += (s, e) => TryConnect();
            }
        }

        public IModel CreateChannel()
        {
            TryConnect();

            if (!IsConnected)            
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            

            return _connection.CreateModel();
        }

    }
}
