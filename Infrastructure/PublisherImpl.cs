using System;
using Tcp.Communication.Messages;

namespace Tcp.Communication.Infrastructure
{
    internal class PublisherImpl : IPublisher
    {
        #region Properties
        public TcpContext Context { get; }
        #endregion

        #region Constructor
        public PublisherImpl(TcpContext context)
        {
            Context = context;
        }

        #endregion

        public PublishedMessage Send(object message)
        {
            string serializedMsg = null;
            if (Context.Serializer != null)
                serializedMsg = Context.Serializer.Serialize(message);

            int byteCount;
            SendIntl(serializedMsg, out byteCount);
            var publishedMessage = new PublishedMessage(message)
            {
                SerializedMessage = serializedMsg,
                SendTime = DateTime.Now,
                ByteCount = byteCount
            };

            return publishedMessage;
        }

        #region Helper Methods

        private void SendIntl(string message, out int byteCount)
        {
            var length = Context.Encoding.GetByteCount(message);
            var fullMsg = string.Concat(length.ToString().PadLeft(Context.PreambleLength, '0'), message);
            byteCount = Send(Context.Encoding.GetBytes(fullMsg));
        }

        private int Send(byte[] messageBytes)
        {
            Context.Stream.Send(messageBytes);
            return messageBytes.Length;
        }

        #endregion
    }
}
