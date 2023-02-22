using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace NewTask
{
    class Program
    {
        private static void Main(string[] args)
        {
            NewTaskMessage();
        }

        public static void NewTaskMessage()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "competing_consumer_pattern", durable: false, exclusive: false, autoDelete: false, null);

            List<string> messages = new List<string>()
            {
                "message 1",
                "message 2",
                "message 3",
                "message 4",
                "message 5",
                "message 6",
                "message 7",
                "message 8",
            };

            foreach (string message in messages)
            {
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: string.Empty, routingKey: "competing_consumer_pattern", null,
                    body: body);
                Console.WriteLine($" [x] Sent {message}");

            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
