namespace Vorbote
{
    using System;
    using System.Text;
    using System.Net;
    using System.Net.Sockets;
    using System.IO;
    using Vorbote.Models;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Authentication;
    using MimeKit;
    using Accounts;
    using System.Threading.Tasks;
    using System.Diagnostics;

    public class Server
    {
        public int Port { get; private set; }

        public IPAddress Address { get; private set; }

        public string HostName { get; private set; }

        public bool TlsRequired { get; private set; }

        public Server()
        {

        }

        public Server(bool requireTls)
        {
            this.TlsRequired = requireTls;
        }

        public Server(string ipAddress, string hostname, int port, bool tlsRequired)
        {
            
        }

        public void Init()
        {
            Port = 443;
            Address = IPAddress.Parse("127.0.0.1");
            HostName = "localhost";
        }

        public void Start()
        {
            var listener = new TcpListener(Address, Port);
            
            listener.Start();

            TcpClient client;

            while (true)
            {
                Debug.WriteLine("Ready...");
                client = listener.AcceptTcpClient();
                StartSession(client);
            }
        }

        private async Task StartSession(TcpClient client)
        {
            string username = null;
            Debug.WriteLine("Connection accepted!");
            NetworkStream networkStream = client.GetStream();
            bool leaveInnerStreamOpen = true;

            RemoteCertificateValidationCallback validationCallback =
              new RemoteCertificateValidationCallback(ClientValidationCallback);

            LocalCertificateSelectionCallback selectionCallback =
              new LocalCertificateSelectionCallback(ServerCertificateSelectionCallback);

            EncryptionPolicy encryptionPolicy = EncryptionPolicy.AllowNoEncryption;
            SslStream sslStream = new SslStream(networkStream, leaveInnerStreamOpen, validationCallback, selectionCallback, encryptionPolicy);
            ServerSideHandshake(sslStream);

            using (StreamWriter writer = new StreamWriter(sslStream))
            {
                writer.WriteLine("220 localhost SMTP server ready.");
                writer.Flush();

                using (StreamReader reader = new StreamReader(sslStream))
                {
                    string response = reader.ReadLine();
                    MailMessage emailMessage = new MailMessage();
                    emailMessage.Sent = DateTime.UtcNow;

                    if (!response.StartsWith("HELO") && !response.StartsWith("EHLO"))
                    {
                        writer.WriteLine("500 UNKNOWN COMMAND");
                        writer.Flush();
                        networkStream.Close();
                        return;
                    }

                    string remote = response.Replace("HELO", string.Empty).Replace("EHLO", string.Empty).Trim();

                    writer.WriteLine("250-localhost");

                    writer.WriteLine("250 AUTH LOGIN PLAIN CRAM-MD5");

                    writer.Flush();

                    response = reader.ReadLine();

                    if (response.ToUpper() == "AUTH LOGIN")
                    {
                        writer.WriteLine("334 VXNlcm5hbWU6");
                        writer.Flush();
                        var encodedUsername = reader.ReadLine();
                        var data = Convert.FromBase64String( encodedUsername);
                        username = Encoding.UTF8.GetString(data);

                        writer.WriteLine("334 UGFzc3dvcmQ6");
                        writer.Flush();
                        var password = reader.ReadLine();
                        writer.WriteLine("235 OK, GO AHEAD");
                        writer.Flush();
                    }

                    do { 
                    response = reader.ReadLine();

                    if (!response.StartsWith("MAIL FROM:"))
                    {
                        emailMessage.From = response;
                        writer.WriteLine("500 UNKNOWN COMMAND");
                        writer.Flush();
                        networkStream.Close();
                        return;
                    }

                    remote = response.Replace("RCPT TO:", string.Empty).Trim();
                    writer.WriteLine("250 " + remote + " I like that guy too!");
                    writer.Flush();

                    response = reader.ReadLine();

                    if (!response.StartsWith("RCPT TO:"))
                    {
                        emailMessage.To = response;
                        writer.WriteLine("500 UNKNOWN COMMAND");
                        writer.Flush();
                        networkStream.Close();
                        return;
                    }

                    remote = response.Replace("MAIL FROM:", string.Empty).Trim();
                    writer.WriteLine("250 " + remote + " I like that guy!");
                    writer.Flush();

                    response = reader.ReadLine();

                    if (response.Trim() != "DATA")
                    {
                        writer.WriteLine("500 UNKNOWN COMMAND");
                        writer.Flush();
                        networkStream.Close();
                        return;
                    }

                    writer.WriteLine("354 Enter message. When finished, enter \".\" on a line by itself");
                    writer.Flush();

                    int counter = 0;
                    StringBuilder message = new StringBuilder();

                    while ((response = reader.ReadLine().Trim()) != ".")
                    {

                        message.AppendLine(response);
                        counter++;

                        if (counter == 1000000)
                        {
                            networkStream.Close();
                            return; 
                        }
                    }
                        Console.WriteLine("Received message:");
                        writer.WriteLine("250 OK");
                        writer.Flush();

                        response = reader.ReadLine();

                        DocumentDbMailBoxRepositry db = new DocumentDbMailBoxRepositry();
                        await db.Initialize();

                        var mailBox = db.GetMailBox(username);
                        var messageBody = message.ToString();
                        var repo = new MailStorage();
                        var key = repo.Storage(mailBox.id, messageBody);

                        using (var stream = MailStorage.GetStream(messageBody))
                        {
                            var parser = new MimeParser(stream);
                            var mimeMessage = parser.ParseMessage();
                        }

                        if(response.ToUpper() == "RSET")
                        {
                            writer.WriteLine("250 OK");
                            writer.Flush();
                        }
                    }
                    while (response.ToUpper() == "RSET") ;
                    networkStream.Close();
                }
            }

        }

        private  void ServerSideHandshake(SslStream sslStream)
        {
            X509Certificate certificate = GetServerCertificate();

            bool requireClientCertificate = false;
            SslProtocols enabledSslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls;
            bool checkCertificateRevocation = false;
            sslStream.AuthenticateAsServer
              (certificate, requireClientCertificate, enabledSslProtocols, checkCertificateRevocation);
        }

        private X509Certificate GetServerCertificate()
        {
            var cert = new X509Certificate2(@"c:\temp\server.pfx", "ready2go");
            return cert;
        }

        public static X509Certificate ServerCertificateSelectionCallback(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return localCertificates[0];
        }

        private  bool ClientValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            Console.WriteLine("Client's authentication succeeded ...\n");
            return true;
        }
    }
}
