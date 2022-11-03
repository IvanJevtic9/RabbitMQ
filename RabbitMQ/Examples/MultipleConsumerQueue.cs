using RabbitMQ.Client;
using RabbitMQ.Implementation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Examples
{
    public class MultipleConsumerQueueExample
    {
        private static Publisher _publisher;
        private static Consumer _consumer1;
        private static Consumer _consumer2;

        private const string FirstQueue = "basicQueue";
        private const string SecondQueue = "adminQueue";

        public static void MainFunc()
        {
            var source = new CancellationTokenSource();

            InitRabbitMQ();

            Task.Run(async () => await ProduceMessages(source.Token));
            Task.Run(async () => await SubscribeConsumerAsync());

            Task.Delay(100000).GetAwaiter().GetResult();

            source.Cancel();
            Console.WriteLine("Shutting down application ...");

            Thread.Sleep(1000);
        }

        private static void InitRabbitMQ()
        {
            var connectionProvider = new ConnectionProvider("amqp://guest:guest@localhost:5672");

            var exchangeName = "basic";
            var exchangeType = ExchangeType.Topic;

            var routingKeyBasic = "admin.basic";
            var routingKeyAll = "admin.*";

            _publisher = new Publisher(connectionProvider, exchangeName, exchangeType);
            _consumer1 = new Consumer(connectionProvider, FirstQueue, routingKeyBasic, exchangeName, exchangeType);
            _consumer2 = new Consumer(connectionProvider, SecondQueue, routingKeyAll, exchangeName, exchangeType);
        }

        private static async Task ProduceMessages(CancellationToken cancellationToken)
        {
            int i = 0;
            var rand = new Random();

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Shuting down producer ...", Console.ForegroundColor);
                    return;
                }

                var gen = rand.Next(0, 2);
                string routingKey = gen == 1 ? "admin.basic" : "admin.anything";

                var messages = $"[{routingKey}] MESSAGE-{i++}";

                await _publisher.PublishAsync(messages, null, routingKey);

                Thread.Sleep(1000);
            }
        }

        private static async Task SubscribeConsumerAsync()
        {
            await _consumer1.SubscribeAsync(async (message, header) =>
            {
                //Console.WriteLine($"Consumer1 received message: {message}");

                return true;
            });

            await _consumer2.SubscribeAsync(async (message, header) =>
            {
                //Console.WriteLine($"Consumer2 received message: {message}");

                return true;
            });
        }
    }
}
