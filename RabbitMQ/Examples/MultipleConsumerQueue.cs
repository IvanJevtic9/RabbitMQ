using RabbitMQ.Implementation;

namespace RabbitMQ.Examples
{
    public class MultipleConsumerQueueExample
    {
        private static Publisher _publisher;
        private static Consumer _consumer1;
        private static Consumer _consumer2;

        private const string FirstQueue = "basicQueue";
        private const string SecondQueue = "adminQueue";

        public MultipleConsumerQueueExample()
        {
            var connectionProvider = new ConnectionProvider("amqp://guest:guest@localhost:5672");

            _publisher = new Publisher(connectionProvider,);
        }

        public static void MainFunc()
        {

        }
    }
}
