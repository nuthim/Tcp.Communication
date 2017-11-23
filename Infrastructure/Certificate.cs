using System.Security.Authentication;

namespace Tcp.Communication.Infrastructure
{
    public class Certificate
    {
        public string Hash { get; set; }

        public string Name { get; set; }

        public SslProtocols Protocols { get; set; }
    }
}