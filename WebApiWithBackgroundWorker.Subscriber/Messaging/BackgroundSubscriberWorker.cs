using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiWithBackgroundWorker.Subscriber.Messaging
{
    public class BackgroundSubscriberWorker : BackgroundService
    {
        private readonly ISubscriber _subscriber;
        private readonly IMessagesRepository _messagesRepository;
        private readonly ILogger<BackgroundSubscriberWorker> _logger;

        public BackgroundSubscriberWorker(ISubscriber subscriber, IMessagesRepository messagesRepository, ILogger<BackgroundSubscriberWorker> logger)
        {
            _messagesRepository = messagesRepository ?? throw new ArgumentNullException(nameof(messagesRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
            _subscriber.OnMessage += OnMessage;
        }

        private void OnMessage(object sender, Common.Messaging.Message message)
        {
            _logger.LogInformation($"got a new message: {message.Text} at {message.CreationDate}");

            _messagesRepository.Add(message);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _subscriber.Start();

            return Task.CompletedTask;
        }
    }
}
