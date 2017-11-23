using System;
using System.IO;
using System.Net.Sockets;
using Tcp.Communication.Exceptions;

namespace Tcp.Communication.Infrastructure
{
    internal class BasicStream : IStream
    {
        #region Fields
        private readonly NetworkStream _stream;
        private bool _disposed;
        #endregion

        #region Constructor

        public BasicStream(NetworkStream stream)
        {
            _stream = stream;
        }

        #endregion

        protected virtual Stream Stream => _stream;

        public bool HasData => _stream.DataAvailable;

        #region IStream Implementation

        public bool CanRead
        {
            get
            {
                ThrowObjectDisposedException();
                return Stream.CanRead;
            }
        }

        public bool CanWrite
        {
            get
            {
                ThrowObjectDisposedException();
                return Stream.CanWrite;
            }
        }

        public void Send(byte[] messageBytes)
        {
            if (!CanWrite)
                throw new CommunicationException("Stream is unable to write");

            Stream.Write(messageBytes, 0, messageBytes.Length);
            Stream.Flush();
        }

        public int Receive(int size, out byte[] data)
        {
            data = new byte[0];

            if (!CanRead)
                throw new CommunicationException("Stream is unable to read");

            data = new byte[size];
            try
            {
                return Stream.Read(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                throw new CommunicationException("Error reading from stream.", ex);
            }
        }

        #endregion

        #region Cleanup

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Stream?.Dispose();
                GC.SuppressFinalize(this);
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~BasicStream()
        {
            Dispose(false);
        }

        protected void ThrowObjectDisposedException()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        #endregion

    }
}
