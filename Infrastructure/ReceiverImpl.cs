using System;
using System.Threading;
using System.Threading.Tasks;
using Tcp.Communication.Messages;

namespace Tcp.Communication.Infrastructure
{
    internal class ReceiverImpl : IReceiver
    {
        #region Fields
        private Task _task;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        #endregion

        #region Properties
        public TcpContext Context { get; }

        #endregion

        #region Events
        public event Action<object, ReceivedMessage> MessageReceived;
        public event Action<object, Exception> Error;
        #endregion

        #region Constructor

        public ReceiverImpl(TcpContext context)
        {
            Context = context;
        }

        #endregion

        public void Start()
        {
            _task = _task ?? Task.Run(() => Process(), _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _task = null;
        }

        #region Helper Methods

        private void Process()
        {
            while (!_cancellationTokenSource.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(1)))
            {
                try
                {
                    var stream = Context.Stream;
                    if (!stream.HasData)
                        continue;

                    var receivedMessage = GetMessage(stream);
                    if (receivedMessage != null)
                        RaiseEvent(MessageReceived, receivedMessage);
                }
                catch (Exception ex)
                {
                    RaiseEvent(Error, ex);
                }
            }
        }

        private ReceivedMessage GetMessage(IStream stream)
        {
            var receivedTime = DateTime.Now;
            int byteCount;
            var message = ReadMessage(stream, out byteCount);

            object deserializedMsg = null;
            Exception deserializationError = null;
            try
            {
                if (message != null && Context.Serializer != null)
                    deserializedMsg = Context.Serializer.Deserialize(message);
            }
            catch (Exception ex)
            {
                deserializationError = ex;
            }

            return new ReceivedMessage(message)
            {
                DeserializedMessage = deserializedMsg,
                DeserializationError = deserializationError,
                ReceivedTime = receivedTime,
                ByteCount = byteCount
            };
        }

        private string ReadMessage(IStream stream, out int byteCount)
        {
            byteCount = 0;
            
            byte[] lengthBytes;
            var bytesReceived = stream.Receive(Context.PreambleLength, out lengthBytes);
            if (bytesReceived == 0)
                return null;

            byteCount = int.Parse(Context.Encoding.GetString(lengthBytes, 0, bytesReceived));
            byte[] msgBytes;
            bytesReceived = stream.Receive(byteCount, out msgBytes);
            return Context.Encoding.GetString(msgBytes, 0, bytesReceived);
        }

        private void RaiseEvent<T>(Action<object, T> action, T args)
        {
            action?.Invoke(this, args);
        }

        #endregion

    }
}