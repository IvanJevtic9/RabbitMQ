using RabbitMQ.Client;
using RabbitMQ.Implementation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Examples
{
    public class MultipleWorkersExample
    {
        private static Publisher _publisher;
        private static Consumer _worker1;
        private static Consumer _worker2;

        private const string BasicQueueName = "basicQueue";

        public static void MainFunc()
        {
            var tokenSource = new CancellationTokenSource();

            InitRabbitMQ();

            Task.Run(() => SubscrubeAllWorker());
            Task.Run(() => ProduceMessages(tokenSource.Token));

            Task.Delay(10000).GetAwaiter().GetResult();

            tokenSource.Cancel();
            Console.WriteLine("Cancelation token activated");

            Thread.Sleep(2000);
        }

        private static void InitRabbitMQ()
        {
            var connectionProvider = new ConnectionProvider("amqp://guest:guest@localhost:5672");

            var exchangeName = "basic";
            var exchangeType = ExchangeType.Direct;

            var arguments = new Dictionary<string, object>();
            arguments.Add("x-priority", 10);

            var arguments1 = new Dictionary<string, object>();
            arguments1.Add("x-priority", 5);

            _publisher = new Publisher(connectionProvider, exchangeName, exchangeType);
            _worker1 = new Consumer(connectionProvider, BasicQueueName, BasicQueueName, exchangeName, exchangeType, arguments1);
            _worker2 = new Consumer(connectionProvider, BasicQueueName, BasicQueueName, exchangeName, exchangeType, arguments);
        }

        private static async Task ProduceMessages(CancellationToken token)
        {
            int i = 0;

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Shuting down producer ...");
                    return;
                }

                var message = $"MESSAGE-{i}";

                var context = new Dictionary<string, object>();

                await _publisher.PublishAsync(message, context, BasicQueueName);

                Thread.Sleep(2500);
                ++i;
            }
        }

        private static async Task SubscrubeAllWorker()
        {
            await _worker1.SubscribeAsync(async (message, header) =>
            {
                Console.WriteLine($"Worker1 received message: {message}");

                return true;
            });

            await _worker2.SubscribeAsync(async (message, header) =>
            {
                Console.WriteLine($"Worker2 received message: {message}");

                return true;
            });
        }
    }
}
