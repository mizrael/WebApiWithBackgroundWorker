using RabbitMQ.Client;

namespace WebApiWithBackgroundWorker.Common.Messaging
{
    public interface IBusConnection
    {
        bool IsConnected { get; }

        IModel CreateChannel();
    }
}