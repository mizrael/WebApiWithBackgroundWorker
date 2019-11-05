using System.Collections.Generic;
using WebApiWithBackgroundWorker.Common.Messaging;

namespace WebApiWithBackgroundWorker.Subscriber.Messaging
{
    public interface IMessagesRepository
    {
        void Add(Message message);
        IReadOnlyCollection<Message> GetMessages();
    }
}