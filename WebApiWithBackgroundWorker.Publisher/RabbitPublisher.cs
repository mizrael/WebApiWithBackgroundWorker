using RabbitMQ.Client;
using System;
using WebApiWithBackgroundWorker.Common.Messaging;
using System.Text.Json;
using System.Text;

namespace WebApiWithBackgroundWorker.Publisher
{
    public class RabbitPublisher : IDisposable
    {
        private readonly string _exchangeName; 
        
        private readonly IBusConnection _connection;        
        private IModel _channel;
        private readonly IBasicProperties _properties;

        public RabbitPublisher(IBusConnection connection, string exchangeName)
        {
            if (string.IsNullOrWhiteSpace(exchangeName))            
                throw new ArgumentException($"'{nameof(exchangeName)}' cannot be null or whitespace", nameof(exchangeName));
            _exchangeName = exchangeName;

            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _channel = _connection.CreateChannel();
            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Fanout);
            _properties = _channel.CreateBasicProperties();           
        }

        public void Publish(Message message)
        {
            var jsonData = JsonSerializer.Serialize(message);

            var body = Encoding.UTF8.GetBytes(jsonData);            

            _channel.BasicPublish(
                exchange: _exchangeName,
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
