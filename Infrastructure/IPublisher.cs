using Tcp.Communication.Messages;

namespace Tcp.Communication.Infrastructure
{
    internal interface IPublisher
    {
        PublishedMessage Send(object message);
    }
}