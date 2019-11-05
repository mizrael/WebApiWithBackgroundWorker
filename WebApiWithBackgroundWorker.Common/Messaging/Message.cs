using System;

namespace WebApiWithBackgroundWorker.Common.Messaging
{
    public class Message
    {        
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Text { get; set; }
    }
}
