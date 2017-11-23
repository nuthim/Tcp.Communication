using System;

namespace Tcp.Communication.Infrastructure
{
    internal interface IStream : IDisposable
    {
        bool HasData { get; }

        bool CanRead { get; }

        bool CanWrite { get; }

        void Send(byte[] messageBytes);

        int Receive(int size, out byte[] data);
    }
}