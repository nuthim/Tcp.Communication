namespace Tcp.Communication.Infrastructure
{
    internal static class ChannelFactory
    {
        public static IChannel GetChannel(TcpContext context)
        {
            var channel = new TcpChannelImpl(context);
            if (!channel.IsConnected)
                channel.Connect();

            return channel;
        }
    }
}