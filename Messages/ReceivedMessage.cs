using System;

namespace Tcp.Communication.Messages
{
    public class ReceivedMessage
    {
        public DateTime ReceivedTime { get; internal set; }

        public string Message { get; }

        public int ByteCount { get; internal set; }

        public object DeserializedMessage { get; internal set; }

        public Exception DeserializationError { get; internal set; }

        public ReceivedMessage(string message)
        {
            Message = message;
        }
    }
}