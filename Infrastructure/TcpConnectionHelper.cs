using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Tcp.Communication.Infrastructure
{
    internal static class TcpConnectionHelper
    {
        public static IEnumerable<TcpConnectionInformation> GetAllConnections()
        {
            return IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
        }

        public static TcpConnectionInformation GetConnection(EndPoint localEndPoint, EndPoint remoteEndPoint)
        {
            return GetAllConnections().SingleOrDefault(x => x.LocalEndPoint.Equals(localEndPoint) && x.RemoteEndPoint.Equals(remoteEndPoint)); ;
        }

        public static IEnumerable<TcpConnectionInformation> GetLocalConnections(EndPoint localEndPoint)
        {
            return GetAllConnections().Where(x => x.LocalEndPoint.Equals(localEndPoint)); ;
        }

        public static IEnumerable<TcpConnectionInformation> GetRemoteConnections(EndPoint remoteEndPoint)
        {
            return GetAllConnections().Where(x => x.RemoteEndPoint.Equals(remoteEndPoint)); ;
        }
    }
}
