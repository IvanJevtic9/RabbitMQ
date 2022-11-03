using System.Collections.Generic;

namespace RabbitMQ.Implementation
{
    public class Consumer : MessageConsumer
    {
        public Consumer(IConnectionProvider connectionProvider, string queueName, string routingKey, string exchangeName, string exchangeType, IDictionary<string, object> arguments = null)
            : base(connectionProvider, queueName, routingKey, exchangeName, exchangeType, arguments)
        { }
    }
}
