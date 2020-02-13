using System.Threading.Tasks;
using System.Threading.Channels;
using System.Threading;
using System;
using WebApiWithBackgroundWorker.Common.Messaging;
using Microsoft.Extensions.Logging;

namespace WebApiWithBackgroundWorker.Subscriber.Messaging
{
    public class Consumer : IConsumer
    {
        private readonly ChannelReader<Message> _reader;
        private readonly ILogger<Consumer> _logger;

        private readonly IMessagesRepository _messagesRepository;
        private readonly int _instanceId;
        private static readonly Random Random = new Random();

        public Consumer(ChannelReader<Message> reader, ILogger<Consumer> logger, int instanceId, IMessagesRepository messagesRepository)
        {
            _reader = reader;
            _instanceId = instanceId;
            _logger = logger;
            _messagesRepository = messagesRepository;
        }

        public async Task BeginConsumeAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Consumer {_instanceId} > starting");

            try
            {
                await foreach (var message in _reader.ReadAllAsync(cancellationToken))
                {
                    _logger.LogInformation($"CONSUMER ({_instanceId})> Received message {message.Id} : {message.Text}");
                    await Task.Delay(500, cancellationToken);
                    _messagesRepository.Add(message);
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning($"Consumer {_instanceId} > forced stop");
            }

            _logger.LogInformation($"Consumer {_instanceId} > shutting down");
        }

    }
}
