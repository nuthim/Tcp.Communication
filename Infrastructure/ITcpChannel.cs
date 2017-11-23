using System.Net;
using System.Net.NetworkInformation;

namespace Tcp.Communication.Infrastructure
{
    internal interface ITcpChannel : IChannel
    {
        TcpState State { get; }

        EndPoint RemoteEndPoint { get; }

        EndPoint LocalEndPoint { get; }
    }
}