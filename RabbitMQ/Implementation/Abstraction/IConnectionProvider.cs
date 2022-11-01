using RabbitMQ.Client;
using System;

namespace RabbitMQ.Implementation
{
    public interface IConnectionProvider : IDisposable
    {
        IConnection GetConnection();
    }
}
