using System.Collections.Generic;

namespace RabbitMQ.Implementation
{
    public class Consumer : MessageConsumer
    {
        public Consumer(IConnectionProvider connectionProvider, string queueName, string exchangeName, string exchangeType, IDictionary<string, object> arguments = null)
            : base(connectionProvider, queueName, exchangeName, exchangeType, arguments)
        { }
    }
}
