namespace Vorbote
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;

    public class SslTransport : TcpTransport
    {
        private SslStream _sslStream;

        public SslTransport(NetworkStream networkStream) : base(networkStream)
        {

        }

        protected override void Initialize()
        {
            _sslStream = CreateSslStream(_networkStream, out _streamReader, out _streamWriter);
        }

        public override void Dispose()
        {
            if (_sslStream != null)
            {
                _sslStream.Dispose();
                _sslStream = null;
            }

            base.Dispose();
        }

        private SslStream CreateSslStream(NetworkStream networkStream, out StreamReader reader, out StreamWriter writer)
        {
            RemoteCertificateValidationCallback validationCallback =
                          new RemoteCertificateValidationCallback(ClientValidationCallback);

            LocalCertificateSelectionCallback selectionCallback =
              new LocalCertificateSelectionCallback(ServerCertificateSelectionCallback);

            bool leaveInnerStreamOpen = true;
            EncryptionPolicy encryptionPolicy = EncryptionPolicy.AllowNoEncryption;
            SslStream sslStream = new SslStream(networkStream, leaveInnerStreamOpen, validationCallback, selectionCallback, encryptionPolicy);
            ServerSideHandshake(sslStream, false, false);

            writer = new StreamWriter(sslStream);
            reader = new StreamReader(sslStream);

            return sslStream;
        }

        private static void ServerSideHandshake(SslStream sslStream, bool requireClientCertificate, bool checkCertificateRevocation)
        {
            X509Certificate certificate = null;
            SslProtocols enabledSslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls;
            sslStream.AuthenticateAsServer(certificate, requireClientCertificate, enabledSslProtocols, checkCertificateRevocation);
        }

        private static X509Certificate ServerCertificateSelectionCallback(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return localCertificates[0];
        }

        private bool ClientValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
