using System.Threading.Tasks;
using WebApiWithBackgroundWorker.Common.Messaging;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace WebApiWithBackgroundWorker.Subscriber.Messaging
{
    public class Producer : IProducer
    {
        private readonly ChannelWriter<Message> _writer;
        private readonly ILogger<Producer> _logger;

        public Producer(ChannelWriter<Message> writer, ILogger<Producer> logger)
        {
            _writer = writer;
            _logger = logger;
        }

        public async Task PublishAsync(Message message, CancellationToken cancellationToken = default)
        {
            await _writer.WriteAsync(message, cancellationToken);
            _logger.LogInformation($"Producer > published message {message.Id} '{message.Text}'");
        }
    }
}
