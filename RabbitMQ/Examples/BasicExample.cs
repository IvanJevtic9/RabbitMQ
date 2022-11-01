using RabbitMQ.Client;
using RabbitMQ.Implementation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Examples
{
    public class BasicExample
    {
        private static Publisher _publisher;
        private static Consumer _consumer;

        private const string BasicQueueName = "basicQueue";

        public static void MainFunc()
        {
            var tokenSource = new CancellationTokenSource();

            InitRabbitMQ();

            Task.Run(() => ProduceMessages(tokenSource.Token));
            Task.Run(() => ConsumeMessages(tokenSource.Token));

            Task.Delay(100000).GetAwaiter().GetResult();

            tokenSource.Cancel();

            Console.WriteLine("Cancelation token activated");

            Thread.Sleep(2000);
        }

        private static void InitRabbitMQ()
        {
            var connectionProvider = new ConnectionProvider("amqp://guest:guest@localhost:5672");

            var exchangeName = "basic";
            var exchangeType = ExchangeType.Topic;

            _publisher = new Publisher(connectionProvider, exchangeName, exchangeType);
            _consumer = new Consumer(connectionProvider, BasicQueueName, exchangeName, exchangeType);
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
                context.Add("param1", "Value1");
                context.Add("param2", 1000);

                await _publisher.PublishAsync(message, context, BasicQueueName);

                Thread.Sleep(2500);
                ++i;
            }
        }

        private static async Task ConsumeMessages(CancellationToken cancellationToken)
        {
            await _consumer.SubscribeAsync(async (message, header) =>
            {
                Console.WriteLine(header["param1"].ToString());
                Console.WriteLine(header["param2"]);

                return true;
            });
        }
    }
}
