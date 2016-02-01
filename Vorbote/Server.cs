namespace Vorbote
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Diagnostics;

    public class Server
    {
        public Server()
        {

        }

        public Server(bool requireTls)
        {
            this.TlsRequired = requireTls;
        }

        public Server(IPAddress ipAddress, string hostname, int port, bool tlsRequired)
        {
            Port = port;
            Address = ipAddress;
            HostName = hostname;
            TlsRequired = tlsRequired;
        }

        public int Port { get; private set; }

        public IPAddress Address { get; private set; }

        public string HostName { get; private set; }

        public bool TlsRequired { get; private set; }

        public void Start()
        {
            var listener = new TcpListener(Address, Port);
            listener.Start();

            while (true)
            {
                Debug.WriteLine("Ready...");
                TcpClient client = listener.AcceptTcpClient();
                SecureSmtpSession session = new SecureSmtpSession(client);
                session.StartSessionAsync();
            }
        }
    }
}
