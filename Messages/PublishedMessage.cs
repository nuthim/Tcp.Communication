using System;

namespace Tcp.Communication.Messages
{
    public class PublishedMessage
    {
        public DateTime SendTime { get; internal set; }

        public object Message { get; internal set; }

        public string SerializedMessage { get; internal set; }

        public int ByteCount { get; internal set; }

        public PublishedMessage(object message)
        {
            Message = message;
        }
    }
}