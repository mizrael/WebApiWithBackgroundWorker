using System;
using WebApiWithBackgroundWorker.Common.Messaging;

namespace WebApiWithBackgroundWorker.Subscriber.Messaging
{
    public class RabbitSubscriberEventArgs : EventArgs{
        public RabbitSubscriberEventArgs(Message message){
            this.Message = message;
        }

        public Message Message{get;}
    }
}
