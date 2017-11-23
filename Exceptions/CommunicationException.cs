using System;


namespace Tcp.Communication.Exceptions
{
    public class CommunicationException : Exception
    {
        public CommunicationException(string message): base(message)
        {
        }

        public CommunicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
