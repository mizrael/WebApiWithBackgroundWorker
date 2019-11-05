using System;
using WebApiWithBackgroundWorker.Common.Messaging;

namespace WebApiWithBackgroundWorker.Subscriber.Messaging
{
    public interface ISubscriber
    {
        void Start();
        event EventHandler<Message> OnMessage;
    }
}
