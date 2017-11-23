using System;

namespace Tcp.Communication.Infrastructure
{
    internal interface IChannel : IDisposable
    {
        bool IsConnected { get; }

        void Connect();

        IStream Stream { get; }
    }
}