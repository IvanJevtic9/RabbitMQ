using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Implementation
{
    public abstract class MessageConsumer : IMessageConsumer
    {
        private readonly IModel _channel;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly string _routingKey;

        protected bool _disposed;

        public MessageConsumer(IConnectionProvider connectionProvider, string queueName, string routingKey, string exchangeName, string exchangeType, IDictionary<string, object> arguments = null)
        {
            _queueName = queueName;
            _exchangeName = exchangeName;
            _routingKey = routingKey;
            _channel = connectionProvider.GetConnection().CreateModel();
            _channel.ExchangeDeclare(exchangeName, exchangeType, durable: true, autoDelete: false, arguments);
            _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, null);
            _channel.QueueBind(queueName, exchangeName, routingKey);
        }

        public virtual async Task SubscribeAsync(Func<string, IDictionary<string, object>, Task<bool>> callback)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                bool success = await callback.Invoke(message, args.BasicProperties.Headers);

                if (success)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Exchange: {_exchangeName} received message. Queue name: {_queueName}. Routing key: {_routingKey}. Message: {message}", 
                        Console.ForegroundColor);
                    _channel.BasicAck(args.DeliveryTag, false);
                }
            };

            _channel.BasicConsume(_queueName, false, consumer);
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
