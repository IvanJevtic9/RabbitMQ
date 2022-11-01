using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitMQ.Implementation
{
    public interface IMessagePublisher : IDisposable
    {
        Task PublishAsync(string message, IDictionary<string, object> context, string queueName);
    }
}
