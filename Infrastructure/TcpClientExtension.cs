using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Tcp.Communication.Infrastructure
{
    internal static class TcpClientExtension
    {
        public static TcpState GetState(this TcpClient client)
        {
            if (client?.Client == null)
                return TcpState.Closed;

            if (!client.Client.Connected)
                return TcpState.Closed;

            var info = TcpConnectionHelper.GetConnection(client.Client.LocalEndPoint, client.Client.RemoteEndPoint);
            
            return info?.State ?? TcpState.Closed;
        }

        public static bool IsConnected(this TcpClient client)
        {
            return GetState(client) == TcpState.Established;
        }
    }
}