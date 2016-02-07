namespace Vorbote
{
    using System;
    using System.Text;
    using System.Net.Sockets;
    using System.IO;
    using Models;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Authentication;
    using MimeKit;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using Configuration;
    using Mailboxes.SqlServer;

    public class SecureSmtpSession : ISmtpSession, IDisposable
    {
        public NetworkStream _networkStream;
        private SslStream _sslStream;
        private StreamWriter _writer;
        private StreamReader _reader;
        private TcpClient _tcpClient;

        public string Username { get; set; }

        public int Account { get; set; }

        public SecureSmtpSession(TcpClient client)
        {
            _tcpClient = client;
            _networkStream = _tcpClient.GetStream();
        }

        public Task StartSessionAsync()
        {
            return Task.Factory.StartNew(() => StartSession());
        }

        public void Dispose()
        {
            if (_networkStream != null)
            {
                _networkStream.Dispose();
                _networkStream = null;
            }
            if (_sslStream != null)
            {
                _sslStream.Dispose();
                _sslStream = null;
            }
            if (_writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }
        }

        private void Send(params string[] messages)
        {
            foreach (string message in messages)
            { 
                _writer.WriteLine(message);
                Debug.WriteLine(string.Format("SERVER: {0}", message));
            }
            _writer.Flush();
        }

        private void SendFormat(string format, params object[] args )
        {
            string message = string.Format(format, args);
            Send(message);
        }

        private string ReadResponse()
        {
            var response = _reader.ReadLine();
            Debug.WriteLine(string.Format("CLIENT: {0}", response));
            return response;
        }

        private static string GetUsername(string encodedUsername)
        {
            string username;
            var data = Convert.FromBase64String(encodedUsername);
            username = Encoding.UTF8.GetString(data);
            return username;
        }

        private void StartSession()
        {
            Debug.WriteLine("Connection accepted!");

            _sslStream = CreateSslStream(_networkStream);

            SendFormat("220 {0} SMTP server ready.", "localhost");
            string response = ReadResponse();
            if (!response.StartsWith("HELO") && !response.StartsWith("EHLO"))
            {
                Send("500 UNKNOWN COMMAND");
                _networkStream.Close();
                return;
            }
            string remoteServer = response.Replace("HELO", string.Empty).Replace("EHLO", string.Empty).Trim();
            Send("250-localhost");



            Send("250 AUTH LOGIN PLAIN CRAM-MD5");
            response = ReadResponse();
            string username = null;
            if (response.ToUpper() == "AUTH LOGIN")
            {
                Send("334 VXNlcm5hbWU6");

                var encodedUsername = ReadResponse();
                username = GetUsername(encodedUsername);

                Send("334 UGFzc3dvcmQ6");
                var passwordResponse = ReadResponse();
                var password = GetUsername(passwordResponse);

                SessionMailBox = VerifyUser(username, password);
                
                if (SessionMailBox != null)
                {
                    Send("235 OK, GO AHEAD");
                }
                else
                {
                    Send("500 Bad Username or Password");
                    _networkStream.Close();
                    return;
                }
            }

            MailMessage emailMessage = new MailMessage() { Sent = DateTime.UtcNow };

            do
            {
                var senderMessage = ReadResponse();

                if (!senderMessage.StartsWith("MAIL FROM:"))
                {
                    emailMessage.From = response;
                    Send("500 UNKNOWN COMMAND");
                    _networkStream.Close();
                    return;
                }
                else
                {
                    var recipient = senderMessage.Replace("MAIL FROM:", string.Empty).Trim();
                    SendFormat("250 Go ahead with message for {0}", recipient);
                }

                var recipientMessage = ReadResponse();

                do
                {

                    if (!recipientMessage.StartsWith("RCPT TO:"))
                    {
                        emailMessage.To = response;
                        Send("500 UNKNOWN COMMAND");
                        _networkStream.Close();
                        return;
                    }
                    else
                    {
                        var sender = recipientMessage.Replace("MAIL FROM:", string.Empty).Trim();
                        SendFormat("250 {0} I like that guy too!", sender);
                    }

                    recipientMessage = ReadResponse();
                }
                while (recipientMessage.Trim() != "DATA");


                Send("354 Enter message. When finished, enter \".\" on a line by itself");

                int counter = 0;
                StringBuilder message = new StringBuilder();

                while ((response = ReadResponse().Trim()) != ".")
                {

                    message.AppendLine(response);
                    counter++;

                    if (counter == 1000000)
                    {
                        _networkStream.Close();
                        return;
                    }
                }

                Console.WriteLine("Received message:");
                Send("250 OK");

                response = ReadResponse();

                var messageBody = message.ToString();

               string uri = MailStorage.Save(username, messageBody);

                using (var stream = MailStorage.GetStream(messageBody))
                {
                    var parser = new MimeParser(stream);
                    var parsedMessage = parser.ParseMessage();
                    HeaderList headers = parsedMessage.Headers;
                    StringBuilder headerBuilder = new StringBuilder();

                    foreach(var header in headers)
                    {
                        headerBuilder.AppendLine(header.ToString());
                    }
                    var headerText = headerBuilder.ToString();
                    var messageText = parsedMessage.GetTextBody(MimeKit.Text.TextFormat.Text);

                    if(messageText != null)
                    {
                        messageText = messageText.Substring(0, 200);
                    }

                    MailboxMessage mmbm = new MailboxMessage
                    {
                         MailboxId = SessionMailBox.MailBoxId,
                         MessageId = parsedMessage.MessageId,
                         Body = messageText,
                         RawHeader = headerText,
                         MessageUrl = uri,
                         Recieved = DateTime.UtcNow,
                         IsRead = false
                    };

                    SqlServerMailboxRepositry.AddMailMessage(SessionMailBox, mmbm);
                }

                if (response.ToUpper() == "RSET")
                {
                    Send("250 OK");
                }
            }
            while (response.ToUpper() == "RSET");
            {
                _networkStream.Close();
            }
        }

        public MailBox SessionMailBox { get; set; }

        private static MailBox VerifyUser(string username, string password)
        {
            var mailbox = SqlServerMailboxRepositry.GetMailbox(username);
            if (mailbox != null)
            {
                if (password == mailbox.Password)
                    return mailbox;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        private SslStream CreateSslStream(NetworkStream networkStream)
        {
            RemoteCertificateValidationCallback validationCallback =
                          new RemoteCertificateValidationCallback(ClientValidationCallback);

            LocalCertificateSelectionCallback selectionCallback =
              new LocalCertificateSelectionCallback(ServerCertificateSelectionCallback);

            bool leaveInnerStreamOpen = true;
            EncryptionPolicy encryptionPolicy = EncryptionPolicy.AllowNoEncryption;
            SslStream sslStream = new SslStream(networkStream, leaveInnerStreamOpen, validationCallback, selectionCallback, encryptionPolicy);
            ServerSideHandshake(sslStream, false, false);

            _writer = new StreamWriter(sslStream);
            _reader = new StreamReader(sslStream);

            return sslStream;
        }

        private static void ServerSideHandshake(
            SslStream sslStream, 
            bool requireClientCertificate, 
            bool checkCertificateRevocation)
        {
            X509Certificate certificate = SmtpServerConfiguration.SslCertificate;
            SslProtocols enabledSslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls;
            sslStream.AuthenticateAsServer(certificate, requireClientCertificate, enabledSslProtocols, checkCertificateRevocation);
        }

        private static X509Certificate ServerCertificateSelectionCallback(
            object sender, 
            string targetHost, 
            X509CertificateCollection localCertificates, 
            X509Certificate remoteCertificate, 
            string[] acceptableIssuers)
        {
            return localCertificates[0];
        }

        private bool ClientValidationCallback(
            object sender, 
            X509Certificate certificate, 
            X509Chain chain, 
            SslPolicyErrors sslPolicyErrors)
        {
            Debug.WriteLine("Client's authentication succeeded ...\n");
            return true;
        }
    }
}
