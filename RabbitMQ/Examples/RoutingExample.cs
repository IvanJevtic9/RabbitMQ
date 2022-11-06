using RabbitMQ.Client;
using RabbitMQ.Implementation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Examples
{
    public class RoutingExample
    {
        private static Publisher _publisher;
        private static Consumer _consumer1;
        private static MultipleRoutingKeyConsumer _consumer2;

        private const string FirstQueue = "basicQueue";
        private const string SecondQueue = "adminQueue";

        public static void MainFunc()
        {
            var tokenSource = new CancellationTokenSource();

            InitRabbitMQ();

            Task.Run(async () => await ProduceMessagesAsync(tokenSource.Token));
            Task.Run(async () => await RegisterSubscribersAsync(tokenSource.Token));

            Task.Delay(10000).GetAwaiter().GetResult();

            tokenSource.Cancel();
            Console.WriteLine("Shutting down application ...");

            Thread.Sleep(2000);
        }

        private static void InitRabbitMQ()
        {
            var connectionProvider = new ConnectionProvider("amqp://guest:guest@localhost:5672");

            var exchangeName = "direct";
            var exchangeType = ExchangeType.Direct;

            var orangeRoutingKey = "orange";
            var blackRoutingKey = "black";
            var greenRoutingKey = "green";

            _publisher = new Publisher(connectionProvider, exchangeName, exchangeType);
            _consumer1 = new Consumer(connectionProvider, FirstQueue, orangeRoutingKey, exchangeName, exchangeType);
            _consumer2 = new MultipleRoutingKeyConsumer(connectionProvider, SecondQueue, new string[] { blackRoutingKey, greenRoutingKey }, exchangeName, exchangeType);
        }

        private static async Task ProduceMessagesAsync(CancellationToken cancellationToken)
        {
            int i = 0;
            var rand = new Random();

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Shutting down producer ...", Console.ForegroundColor);
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                var gen = rand.Next(0, 4);

                var routingKey = gen switch
                {
                    0 => "orange",
                    1 => "black",
                    _ => "green"
                };

                var messages = $"[{routingKey}] MESSAGE-{i++}";

                await _publisher.PublishAsync(messages, null, routingKey);

                Thread.Sleep(1000);
            }
        }

        private static async Task RegisterSubscribersAsync(CancellationToken cancellationToken)
        {
            await _consumer1.SubscribeAsync(async (message, context) =>
            {
                return true;
            }).ConfigureAwait(false);
            await _consumer2.SubscribeAsync(async (message, context) =>
            {
                return true;
            }).ConfigureAwait(false);
        }
    }
}
