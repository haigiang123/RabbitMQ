using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            Worker();
        }

        public static void Worker()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "competing_consumer_pattern", durable: false, exclusive: false, autoDelete: false, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (a, data) =>
            {
                var message = Encoding.UTF8.GetString(data.Body.ToArray());

                Console.WriteLine($" [x] Received {message}");

                Thread.Sleep(2000);

                channel.BasicAck(deliveryTag: data.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: "competing_consumer_pattern", autoAck: false, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

    }
}
