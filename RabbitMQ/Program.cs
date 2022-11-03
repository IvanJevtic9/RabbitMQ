using RabbitMQ.Examples;

namespace RabbitMQ
{
    public class Program
    {
        // Before starting the program , run rabbitMQ server (docker)
        // amqp://guest:guest@localhost:5672

        // Exchagnge Types:
        // Topic - rabbitMQ exchange type sends messages to queues depending on wildcard
        //         matches between the routing key and the queue binding’s routing pattern
        // Direct - A direct exchange delivers messages to queues based on the message routing key.
        //          A direct exchange is ideal for the unicast routing of messages (although they can be used for multicast routing as well).
        //         Here is how it works: A queue binds to the exchange with a routing key K.
        //
        //

        public static void Main(string[] args)
        {
            //BasicExample.MainFunc();
            //MultipleWorkersExample.MainFunc();
            MultipleConsumerQueueExample.MainFunc();

        }
    }
}
