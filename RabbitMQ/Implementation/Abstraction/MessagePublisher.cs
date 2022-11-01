using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Implementation
{
    public abstract class MessagePublisher : IMessagePublisher
    {
        private readonly IModel _channel;
        private readonly string _exchangeName;

        protected bool _disposed;

        public MessagePublisher(IConnectionProvider connectionProvider, string exchangeName, string exchangeType, IDictionary<string, object> arguments = null)
        {
            _exchangeName = exchangeName;
            _channel = connectionProvider.GetConnection().CreateModel();
            _channel.ExchangeDeclare(exchangeName, exchangeType, durable: true, autoDelete: false, arguments);
        }

        public virtual async Task PublishAsync(string message, IDictionary<string, object> context, string queueName)
        {
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.Headers = context;

            _channel.BasicPublish(_exchangeName, queueName, properties, body);

            Console.WriteLine($"Exchange: {_exchangeName} sent message. Routing key: {queueName}. Message: {message}");
        }

        public virtual void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _channel?.Close();
            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
