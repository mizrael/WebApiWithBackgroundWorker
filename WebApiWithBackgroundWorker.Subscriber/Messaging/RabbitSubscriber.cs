using System;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WebApiWithBackgroundWorker.Common.Messaging;
using Microsoft.Extensions.Logging;

namespace WebApiWithBackgroundWorker.Subscriber.Messaging
{
    public record RabbitSubscriberOptions(string ExchangeName, string QueueName, string DeadLetterExchangeName, string DeadLetterQueue);

    public class RabbitSubscriber : ISubscriber, IDisposable
    {
        private readonly IBusConnection _connection;
        private IModel _channel;
        private readonly ILogger<RabbitSubscriber> _logger;
        
        private readonly RabbitSubscriberOptions _options;

        public RabbitSubscriber(IBusConnection connection, RabbitSubscriberOptions options, ILogger<RabbitSubscriber> logger)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private void InitChannel()
        {
            _channel?.Dispose();
            
            _channel = _connection.CreateChannel();

            _channel.ExchangeDeclare(exchange: _options.DeadLetterExchangeName, type: ExchangeType.Fanout);
            _channel.QueueDeclare(queue: _options.DeadLetterQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            _channel.QueueBind(_options.DeadLetterQueue, _options.DeadLetterExchangeName, routingKey: string.Empty, arguments: null);

            _channel.ExchangeDeclare(exchange: _options.ExchangeName, type: ExchangeType.Fanout);
                        
            _channel.QueueDeclare(queue: _options.QueueName,
                durable: false,
                exclusive: false,
                autoDelete: true,
                arguments: null);

            _channel.QueueBind(_options.QueueName, _options.ExchangeName, string.Empty, null);

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
            
            _channel.BasicConsume(queue: _options.QueueName, autoAck: false, consumer: consumer);
        }

        private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            var consumer = sender as IBasicConsumer;
            var channel = consumer?.Model ?? _channel;

            Message message = null;
            try
            {
                var body = Encoding.UTF8.GetString(eventArgs.Body.Span);
                message = JsonSerializer.Deserialize<Message>(body);
                await this.OnMessage(this, new RabbitSubscriberEventArgs(message));
            }
            catch(Exception ex)
            {
                var errMsg = (message is null) ? $"an error has occurred while processing a message: {ex.Message}"
                    : $"an error has occurred while processing message '{message.Id}': {ex.Message}";
                _logger.LogError(ex, errMsg);

                if (eventArgs.Redelivered)
                    channel.BasicReject(eventArgs.DeliveryTag, requeue: false);
                else
                    channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
            }            
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
