using System;
using Tcp.Communication.Messages;

namespace Tcp.Communication.Infrastructure
{
    internal interface IReceiver
    {
        event Action<object, ReceivedMessage> MessageReceived;

        event Action<object, Exception> Error;

        void Start();

        void Stop();
    }
}
