using RabbitMQ.Client;
using System;

namespace RabbitMQ.Implementation
{
    public class ConnectionProvider : IConnectionProvider
    {
        private readonly ConnectionFactory _factory;
        private IConnection _connection;

        protected bool _disposed;

        public ConnectionProvider(string uri)
        {
            _factory = new()
            {
                Uri = new Uri(uri)
            };

            try
            {
                _connection = _factory.CreateConnection();
                Console.WriteLine("RabbitMQ Connection established.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RabbitMQ connection failed. \n{ex}");
            }
        }

        public virtual IConnection GetConnection()
        {
            while (_connection == null || !_connection.IsOpen)
            {
                try
                {
                    _connection = _factory.CreateConnection();
                    _connection.ConnectionShutdown += (sender, args) => { Console.WriteLine($"Connection shutdown"); };
                    Console.WriteLine("RabbitMQ Connection established.");
                }
                catch (Exception)
                {
                    Console.WriteLine("RabbitMQ connection failed.");
                }
            }

            return _connection;
        }

        public virtual void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _connection.Close();
            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
