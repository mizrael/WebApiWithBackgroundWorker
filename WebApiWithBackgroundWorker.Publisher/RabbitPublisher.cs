using RabbitMQ.Client;
using System;
using WebApiWithBackgroundWorker.Common.Messaging;
using System.Text.Json;
using System.Text;

namespace WebApiWithBackgroundWorker.Publisher
{
    public class RabbitPublisher
    {
        private readonly IBusConnection _connection;
        private const string ExchangeName = "messages";

        public RabbitPublisher(IBusConnection persistentConnection)
        {
            _connection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));            
        }

        public void Publish(Message message)
        {           
            using (var channel = _connection.CreateChannel())
            {
                channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);

                var jsonData = JsonSerializer.Serialize(message);

                var body = Encoding.UTF8.GetBytes(jsonData);

                var properties = channel.CreateBasicProperties();

                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: string.Empty,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            }
        }
    }
}
