using RabbitMQ.Client;

namespace WebApiWithBackgroundWorker.Common.Messaging
{
    public interface IRabbitPersistentConnection
    {
        bool IsConnected { get; }

        IModel CreateChannel();
    }
}