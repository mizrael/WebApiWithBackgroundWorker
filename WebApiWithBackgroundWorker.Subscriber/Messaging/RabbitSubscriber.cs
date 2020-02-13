using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WebApiWithBackgroundWorker.Common.Messaging;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace WebApiWithBackgroundWorker.Subscriber.Messaging
{
    public class RabbitSubscriber : ISubscriber, IDisposable
    {
        private readonly IBusConnection _connection;
        private IModel _channel;
        private QueueDeclareOk _queue;

        private const string ExchangeName = "messages";

        public RabbitSubscriber(IBusConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        private void InitChannel()
        {
            _channel?.Dispose();
            
            _channel = _connection.CreateChannel();

            _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);
            
            // since we're using a Fanout exchange, we don't specify the name of the queue
            // but we let Rabbit generate one for us. This also means that we need to store the
            // queue name to be able to consume messages from it
            _queue = _channel.QueueDeclare(queue: string.Empty,
                durable: false,
                exclusive: false,
                autoDelete: true,
                arguments: null);

            _channel.QueueBind(_queue.QueueName, ExchangeName, string.Empty, null);

            _channel.CallbackException += (sender, ea) =>
            {
                InitChannel();
                InitSubscription();
            };
        }

        private void InitSubscription()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            
            consumer.Received += OnMessageReceivedAsync;
            
            _channel.BasicConsume(queue: _queue.QueueName, autoAck: false, consumer: consumer);
        }

        private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            var body = Encoding.UTF8.GetString(eventArgs.Body);
            var message = JsonSerializer.Deserialize<Message>(body);
            await this.OnMessage(this, new RabbitSubscriberEventArgs(message));
        }

        public event AsyncEventHandler<RabbitSubscriberEventArgs> OnMessage;

        public void Start()
        {
            InitChannel();
            InitSubscription();
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
