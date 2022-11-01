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
        // 
        //
        //

        public static void Main(string[] args)
        {
            BasicExample.MainFunc();
        }
    }
}
