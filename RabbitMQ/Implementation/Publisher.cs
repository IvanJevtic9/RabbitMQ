using System.Collections.Generic;

namespace RabbitMQ.Implementation
{
    public class Publisher : MessagePublisher
    {
        public Publisher(IConnectionProvider connectionProvider, string exchangeName, string exchangeType, IDictionary<string, object> arguments = null)
            : base(connectionProvider, exchangeName, exchangeType, arguments)
        { }
    }
}
