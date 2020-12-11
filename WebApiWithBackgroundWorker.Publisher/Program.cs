using System;
using WebApiWithBackgroundWorker.Common.Messaging;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;

namespace WebApiWithBackgroundWorker.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            
            var config = builder.Build();

            var rabbitConfig = config.GetSection("RabbitMQ");
            var connectionFactory = new ConnectionFactory()
            {
                HostName = rabbitConfig["HostName"],
                UserName = rabbitConfig["UserName"],
                Password = rabbitConfig["Password"],
                Port = AmqpTcpEndpoint.UseDefaultPort
            };

            var connection = new RabbitPersistentConnection(connectionFactory);
            var publisher = new RabbitPublisher(connection, rabbitConfig["Exchange"]);                       

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("type your messages and press ENTER to send. Press CTRL+C to quit.");

                var text = Console.ReadLine();

                try
                {
                    var message = new Message()
                    {
                        Id = Guid.NewGuid(),
                        CreationDate = DateTime.UtcNow,
                        Text = text
                    };
                    publisher.Publish(message);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("message sent!");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"an error has occurred while sending the message: {ex.Message}");
                }

                Console.ResetColor();
            }
        }
    }
}
