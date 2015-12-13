﻿namespace Vorbote
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

    public class SecureSmtpSession
    {
        public NetworkStream _networkStream;
        private SslStream _sslStream;
        private StreamWriter _writer;
        private StreamReader _reader;

        public SecureSmtpSession(TcpClient client)
        {
            _networkStream = client.GetStream();
        }

        public Task StartSessionAsync()
        {
            return Task.Factory.StartNew(() => StartSession(_networkStream));
        }

        private void Send(params string[] messages)
        {
            foreach (string message in messages)
            { 
                _writer.WriteLine(message);
            }
            _writer.Flush();
        }

        private string ReadResponse()
        {
            return _reader.ReadLine();
        }

        private bool  ValidateRecipient()
        {
            var senderMessage = ReadResponse();

            if (!senderMessage.StartsWith("MAIL FROM:"))
            {
                Send("500 UNKNOWN COMMAND");
                _networkStream.Close();
                return;
            }
            else
            {

                var recipient = response.Replace("RCPT TO:", string.Empty).Trim();
                string recipientAcknowledgement = string.Format("250 {0} I like that guy too!", recipient);
                Send(recipientAcknowledgement);
            }
        }

        private bool ValidateRecipientMessage(string senderMessage)
        {
            if (!senderMessage.ToUpper().StartsWith("MAIL FROM:"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void StartSession(NetworkStream networkStream)
        {
            Debug.WriteLine("Connection accepted!");

            _sslStream = CreateSslStream(networkStream);

            Send("220 localhost SMTP server ready.");
            string response = ReadResponse();

            string username = null;

            if (!response.StartsWith("HELO") && !response.StartsWith("EHLO"))
            {
                Send("500 UNKNOWN COMMAND");
                networkStream.Close();
                return;
            }

            string remoteServer = response.Replace("HELO", string.Empty).Replace("EHLO", string.Empty).Trim();
            Send("250-localhost", "250 AUTH LOGIN PLAIN CRAM-MD5");

            response = ReadResponse();

            if (response.ToUpper() == "AUTH LOGIN")
            {
                Send("334 VXNlcm5hbWU6");

                var encodedUsername = ReadResponse();
                var data = Convert.FromBase64String(encodedUsername);
                username = Encoding.UTF8.GetString(data);

                Send("334 UGFzc3dvcmQ6");
                var password = ReadResponse();
                Send("235 OK, GO AHEAD");
            }

            MailMessage emailMessage = new MailMessage();
            emailMessage.Sent = DateTime.UtcNow;

            do
            {
                var senderMessage = ReadResponse();

                if (!response.StartsWith("MAIL FROM:"))
                {
                    emailMessage.From = response;
                    Send("500 UNKNOWN COMMAND");
                    networkStream.Close();
                    return;
                }
                else
                {

                    var recipient = response.Replace("RCPT TO:", string.Empty).Trim();
                    string recipientAcknowledgement = string.Format("250 {0} I like that guy too!", recipient);
                    Send(recipientAcknowledgement);
                }

                response = ReadResponse();

                if (!response.StartsWith("RCPT TO:"))
                {
                    emailMessage.To = response;
                    Send("500 UNKNOWN COMMAND");
                    networkStream.Close();
                    return;
                }

                var sender = response.Replace("MAIL FROM:", string.Empty).Trim();
                string senderAcknowledgement = string.Format("250 {0} I like that guy too!", sender);
                Send(senderAcknowledgement);

                response = ReadResponse();

                if (response.Trim() != "DATA")
                {
                    Send("500 UNKNOWN COMMAND");
                    networkStream.Close();
                    return;
                }

                Send("354 Enter message. When finished, enter \".\" on a line by itself");

                int counter = 0;
                StringBuilder message = new StringBuilder();

                while ((response = ReadResponse().Trim()) != ".")
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
                Send("250 OK");

                response = ReadResponse();

                var messageBody = message.ToString();


                using (var stream = MailStorage.GetStream(messageBody))
                {
                    var parser = new MimeParser(stream);
                    var mimeMessage = parser.ParseMessage();
                }

                if (response.ToUpper() == "RSET")
                {
                    Send("250 OK");
                }
            }
            while (response.ToUpper() == "RSET");
                    networkStream.Close();
                


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
            ServerSideHandshake(sslStream);

            _writer = new StreamWriter(_sslStream);
            _reader = new StreamReader(_sslStream);

            return sslStream;
        }

        private void ServerSideHandshake(SslStream sslStream)
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

        private bool ClientValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            Console.WriteLine("Client's authentication succeeded ...\n");
            return true;
        }
    }
}
