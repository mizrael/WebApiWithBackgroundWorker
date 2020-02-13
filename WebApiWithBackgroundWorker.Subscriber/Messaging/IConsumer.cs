using System.Threading.Tasks;
using System.Threading;

namespace WebApiWithBackgroundWorker.Subscriber.Messaging
{
    public interface IConsumer
    {
        Task BeginConsumeAsync(CancellationToken cancellationToken = default);
    }
}
