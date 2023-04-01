using RabbitMQ.Client;
using System;
using System.Text;
using RabbitMQ.Client.Events;

namespace ReceiveLogDirect
{
    class Program
    {
        static void Main(string[] args)
        {
            ReceiveLogDirect(args);
        }

        public static void ReceiveLogDirect(string[] args)
        {
            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);
            var queueName = channel.QueueDeclare().QueueName;

            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
                Environment.GetCommandLineArgs()[0]);
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
                Environment.ExitCode = 1;
                return;
            }

            foreach (var s in args)
            {
                channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: s, null);
            }

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (m, data) =>
            {
                var aaa = m;
                var body = data.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = data.RoutingKey;
                Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
