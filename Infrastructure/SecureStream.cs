using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace Tcp.Communication.Infrastructure
{
    internal class SecureStream : BasicStream
    {
        #region Fields
        private SslStream _sslStream;
        #endregion

        #region Constructor

        public SecureStream(NetworkStream stream, Certificate certificate) : base(stream)
        {
            Certificate = certificate;
        }

        #endregion

        public Certificate Certificate { get; }

        protected override Stream Stream => _sslStream ?? (_sslStream = GetStream());

        #region Helper Methods

        private SslStream GetStream()
        {
            var innerStream = base.Stream;

            var sslStream = new SslStream(
                innerStream,
                false,
                ValidateServerCertificate,
                null);

            sslStream.AuthenticateAsClient(Certificate.Name, null, Certificate.Protocols, false);

            return sslStream;
        }

        private bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                // we can perform additional certificate checks here 
                return true;
            }

            // Do not allow this client to communicate with unauthenticated servers.
            // log the certificate error into the error log.
            var s = "SSL certificate error: ";
            s += sslPolicyErrors;



            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) != 0)
            {
                // if name mismatch error is reported, and the stored name is not empty
                // then the remote certificate is not acceptable.

                string s2 = "CN=";
                s2 += Certificate.Name.ToUpper();

                s = certificate.Subject.ToUpper();

                if (Certificate.Name.Length != 0 && (!s.Contains(s2)))
                {
                    Console.WriteLine("SSL certificate name " + certificate.Subject + " unexpected with given certificate " + Certificate.Name + ". Rejecting ...");
                    return false;
                }
            }

            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                // if there is a certificate chain error, then we accept the certificate
                // if its hash matches what we expect, or if stored hash is "-"
                s = certificate.GetCertHashString();

                if (s != null)
                {
                    if ((!s.Equals(Certificate.Hash, StringComparison.OrdinalIgnoreCase)) && Certificate.Hash.Length != 0)
                    {
                        Console.WriteLine("SSL certificate hash " + s + " unexpected with given hash " + Certificate.Hash + ". Rejecting ...");
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

    }
}