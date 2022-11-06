using RabbitMQ.Client;
using System.Collections.Generic;

namespace RabbitMQ.Implementation
{
    public class MultipleRoutingKeyConsumer : MessageConsumer
    {
        public MultipleRoutingKeyConsumer(
            IConnectionProvider connectionProvider,
            string queueName,
            string[] routingKeys,
            string exchangeName,
            string exchangeType,
            IDictionary<string, object> arguments = null)
                : base(connectionProvider, queueName, routingKeys[0], exchangeName, exchangeType, arguments)
        {
            for (int i = 1; i < routingKeys.Length; i++)
            {
                _channel.QueueBind(queueName, exchangeName, routingKeys[i]);
            }
        }
    }
}
