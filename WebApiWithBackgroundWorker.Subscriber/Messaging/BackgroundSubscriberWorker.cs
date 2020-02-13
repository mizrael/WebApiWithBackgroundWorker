using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiWithBackgroundWorker.Subscriber.Messaging
{
    public class BackgroundSubscriberWorker : BackgroundService
    {
        private readonly ISubscriber _subscriber;
        private readonly ILogger<BackgroundSubscriberWorker> _logger;

        private readonly IProducer _producer;
        private readonly IEnumerable<IConsumer> _consumers;

        public BackgroundSubscriberWorker(ISubscriber subscriber, IProducer producer, IEnumerable<IConsumer> consumers, ILogger<BackgroundSubscriberWorker> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
            _subscriber.OnMessage += OnMessageAsync;

            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _consumers = consumers ?? Enumerable.Empty<Consumer>();
        }
 
        private async Task OnMessageAsync(object sender, RabbitSubscriberEventArgs args)
        {
            _logger.LogInformation($"got a new message: {args.Message.Text} at {args.Message.CreationDate}");

            await _producer.PublishAsync(args.Message);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _subscriber.Start();
            
            var consumerTasks = _consumers.Select(c => c.BeginConsumeAsync(stoppingToken));
            await Task.WhenAll(consumerTasks);
        }
    }
}
