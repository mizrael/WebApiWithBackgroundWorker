using RabbitMQ.Client;
using System;
using WebApiWithBackgroundWorker.Common.Messaging;
using System.Text.Json;
using System.Text;

namespace WebApiWithBackgroundWorker.Publisher
{
    public class RabbitPublisher : IDisposable
    {
        private const string ExchangeName = "messages"; 
        
        private readonly IBusConnection _connection;        
        private readonly IModel _channel;
        private readonly IBasicProperties _properties;

        public RabbitPublisher(IBusConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _channel = _connection.CreateChannel();
            _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);
            _properties = _channel.CreateBasicProperties();
        }

        public void Publish(Message message)
        {
            var jsonData = JsonSerializer.Serialize(message);

            var body = Encoding.UTF8.GetBytes(jsonData);            

            _channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: string.Empty,
                mandatory: true,
                basicProperties: _properties,
                body: body);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _channel = null;
        }
    }
}
