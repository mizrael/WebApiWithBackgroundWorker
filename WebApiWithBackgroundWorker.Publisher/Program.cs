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
            
            builder.AddUserSecrets<Program>();
            var config = builder.Build();

            var connectionString = config["rabbit"];

            var connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(connectionString)
            };            
            var connection = new RabbitPersistentConnection(connectionFactory);
            var publisher = new RabbitPublisher(connection);                       

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
