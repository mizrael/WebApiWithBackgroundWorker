using System.Threading.Tasks;
using WebApiWithBackgroundWorker.Common.Messaging;
using System.Threading;

namespace WebApiWithBackgroundWorker.Subscriber.Messaging
{
    public interface IProducer
    {
        Task PublishAsync(Message message, CancellationToken cancellationToken = default);
    }
}
