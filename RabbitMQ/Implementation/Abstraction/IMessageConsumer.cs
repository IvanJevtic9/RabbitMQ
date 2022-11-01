using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitMQ.Implementation
{
    public interface IMessageConsumer : IDisposable
    {
        Task SubscribeAsync(Func<string, IDictionary<string, object>, Task<bool>> callback);
    }
}
