using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tcp.Communication.Infrastructure;
using Tcp.Communication.Messages;

namespace Tcp.Communication
{
    public class TcpContext : IDisposable
    {
        #region Fields
        private bool _disposed;
        private IChannel _channel;
        private readonly IPublisher _publisher;
        private readonly IReceiver _receiver;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly object _syncLock = new object();
        #endregion

        #region Events

        public event Action<object, EventArgs> Disconnected;
        public event Action<object, ReceivedMessage> MessageReceived;
        public event Action<object, Exception> CommunicationError;

        #endregion

        #region Properties

        public IPEndPoint RemoteEndPoint { get; set; }

        public Certificate Certificate { get; set; }

        public int PreambleLength { get; set; }

        public Encoding Encoding { get; set; }

        public ISerializer Serializer { get; set; }

        public bool IsOpen => _channel != null && _channel.IsConnected;

        internal IStream Stream => _channel?.Stream;

        #endregion

        #region Constructor
        public TcpContext()
        {
            _publisher = new PublisherImpl(this);
            _receiver = new ReceiverImpl(this);
            _receiver.MessageReceived += (sender, message) => { RaiseEvent(MessageReceived, this, message); };
            _receiver.Error += (sender, error) => { RaiseEvent(CommunicationError, this, error); };
        }

        #endregion

        public void Open()
        {
            lock (_syncLock)
            {
                ThrowObjectDisposedException();

                if (_channel?.IsConnected == true)
                    return;

                Initialize();
            }
        }

        public void Close()
        {
            Dispose();
        }

        public PublishedMessage Send(object message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return _publisher.Send(message);
        }

        #region Cleanup

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _cancellationTokenSource?.Cancel();
                _receiver.Stop();
                _channel?.Dispose();
            }

            _disposed = true;
        }

        ~TcpContext()
        {
            Dispose(false);
        }

        private void ThrowObjectDisposedException()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        #endregion

        #region Helper Methods

        private void Validate()
        {
            if (RemoteEndPoint == null)
                throw new InvalidOperationException("RemoteEndPoint is not set");
        }

        private void Initialize()
        {
            Validate();

            var channel = _channel ?? (_channel = ChannelFactory.GetChannel(this));
            if (!channel.IsConnected)
                channel.Connect();

            _receiver.Start();

            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => CheckConnected(), _cancellationTokenSource.Token).ContinueWith(task =>
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                    return;

                RaiseEvent(Disconnected, this, EventArgs.Empty);
            });
        }

        private void CheckConnected()
        {
            while (true)
            {
                if (_cancellationTokenSource.IsCancellationRequested || !_channel.IsConnected)
                    break;

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        private static void RaiseEvent<T>(Action<object, T> action,  object sender, T args)
        {
            var invocationList = action?.GetInvocationList();
            if (invocationList == null)
                return;

            foreach (var subscriber in invocationList)
            {
                try
                {
                    subscriber.DynamicInvoke(sender, args);
                }
                catch
                {
                    //Ignore
                }
            }
        }

        #endregion
    }
}
