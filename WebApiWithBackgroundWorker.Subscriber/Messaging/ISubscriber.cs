using System;
using RabbitMQ.Client.Events;
using WebApiWithBackgroundWorker.Common.Messaging;

namespace WebApiWithBackgroundWorker.Subscriber.Messaging
{
    public interface ISubscriber
    {
        void Start();
        event AsyncEventHandler<RabbitSubscriberEventArgs> OnMessage;
    }
}
