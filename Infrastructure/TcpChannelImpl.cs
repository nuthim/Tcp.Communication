using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Tcp.Communication.Infrastructure
{
    internal class TcpChannelImpl : ITcpChannel
    {
        #region Fields
        private bool _disposed;
        private TcpClient _client;
        private IStream _stream;
        #endregion

        #region Properties

        public TcpContext Context { get; }

        public EndPoint RemoteEndPoint
        {
            get
            {
                ThrowObjectDisposedException();
                return !IsConnected ? null : _client.Client.RemoteEndPoint;
            }
        }

        public EndPoint LocalEndPoint
        {
            get
            {
                ThrowObjectDisposedException();
                return !IsConnected ? null : _client.Client.LocalEndPoint;
            }
        }

        public bool IsConnected
        {
            get
            {
                ThrowObjectDisposedException();
                return _client.IsConnected();
            }
        }

        public TcpState State
        {
            get
            {
                ThrowObjectDisposedException();
                return _client.GetState();
            }
        }

        public IStream Stream
        {
            get
            {
                ThrowObjectDisposedException();
                return _stream;
            }
        }

        #endregion

        #region Constructor

        public TcpChannelImpl(TcpContext context)
        {
            Context = context;
        }

        #endregion

        public void Connect()
        {
            ThrowObjectDisposedException();
            Initialize();
        }

        #region Helper Methods

        private void Initialize()
        {
            _client = new TcpClient {NoDelay = true};
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            _client.Connect(Context.RemoteEndPoint);
            _stream = Context.Certificate == null ? new BasicStream(_client.GetStream()) : new SecureStream(_client.GetStream(), Context.Certificate);
        }

        #endregion

        #region Cleanup

        public void Dispose()
        {
            Dispose(true);
        }

        ~TcpChannelImpl()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _client?.Close();
                Stream?.Dispose();
                GC.SuppressFinalize(this);
            }

            _disposed = true;
        }

        private void ThrowObjectDisposedException()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        #endregion
    }
}
