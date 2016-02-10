namespace Vorbote
{
    using MimeKit;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Mailboxes.SqlServer;
    using Models;

    public class SmtpSession
    {
        private readonly Stream _stream;
        private readonly StreamWriter _writer;
        private readonly StreamReader _reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpSession"/> class.
        /// </summary>
        /// <param name="streamWriter"></param>
        public SmtpSession(StreamReader streamReader, StreamWriter streamWriter)
        {
            if(streamWriter == null || streamReader == null)
            {
                throw new ArgumentException();
            }

            _stream = streamWriter.BaseStream;
            _reader = streamReader;
            _writer = streamWriter;
        }

        public string RemoteServer { get; private set; }

        public string RemoteUser { get; private set; }


        private void Send(params string[] messages)
        {
            foreach (string message in messages)
            {
                _writer.WriteLine(message);
                Debug.WriteLine(string.Format("SERVER: {0}", message));
            }
            _writer.Flush();
        }

        private void SendFormat(string format, params object[] args)
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

        private static string DecodeBase64(string encodedUsername)
        {
            string username;
            var data = Convert.FromBase64String(encodedUsername);
            username = Encoding.UTF8.GetString(data);
            return username;
        }

        private void ReadMailMessage()
        {
            var data = String.Empty;

            MailMessage emailMessage = new MailMessage() { Sent = DateTime.UtcNow };

            var senderMessage = ReadResponse();

            if (!senderMessage.StartsWith("MAIL FROM:"))
            {
                Send("500 UNKNOWN COMMAND");
                _stream.Close();
                return;
            }
            else
            {
                var recipient = senderMessage.Replace("MAIL FROM:", string.Empty).Trim();
                SendFormat("250 Go ahead with message for {0}", recipient);
            }

            var recipientMessage = ReadResponse();

            if (!recipientMessage.StartsWith("RCPT TO:"))
            {
                Send("500 UNKNOWN COMMAND");
                _stream.Close();
                return;
            }
            else
            {
                var sender = recipientMessage.Replace("MAIL FROM:", string.Empty).Trim();
                SendFormat("250 {0} I like that guy too!", sender);
            }

            data = ReadResponse();

            if (data.Trim() != "DATA")
            {
                Send("500 UNKNOWN COMMAND");
                _stream.Close();
                return;
            }

            Send("354 Enter message. When finished, enter \".\" on a line by itself");

            int counter = 0;
            StringBuilder message = new StringBuilder();

            string splitData;
            while ((splitData = ReadResponse().Trim()) != ".")
            {

                message.AppendLine(splitData);
                counter++;
            }

            Console.WriteLine("Received message:");
            Send("250 OK");

            

            var messageBody = message.ToString();

            string key = MailStorage.Save(RemoteUser, messageBody);

            using (var stream = MailStorage.GetStream(messageBody))
            {
                var parser = new MimeParser(stream);
                HeaderList headers = parser.ParseHeaders();
                StringBuilder headerBuilder = new StringBuilder();
                foreach (var header in headers)
                {
                    headerBuilder.AppendLine(header.ToString());
                }
                var headerText = headerBuilder.ToString();
                MailStorage.QueueMessage(key, RemoteUser, headerText);
            }
        }

        public void StartSession()
        {
            Debug.WriteLine("Connection accepted!");

            SendFormat("220 {0} SMTP server ready.", "localhost");
            string response = ReadResponse();

            string username = null;

            if (!response.StartsWith("HELO") && !response.StartsWith("EHLO"))
            {
                Send("500 UNKNOWN COMMAND");
                _stream.Close();
                return;
            }

            RemoteServer = response.Replace("HELO", string.Empty).Replace("EHLO", string.Empty).Trim();

            Send("250-localhost", "250 AUTH LOGIN PLAIN CRAM-MD5");

            response = ReadResponse();

            if (response.ToUpper() == "AUTH LOGIN")
            {
                Send("334 VXNlcm5hbWU6");

                var encodedUsername = ReadResponse();
                username = DecodeBase64(encodedUsername);

                Send("334 UGFzc3dvcmQ6");
                var passwordResponse = ReadResponse();
                var password = DecodeBase64(passwordResponse);

                RemoteUser = SqlServerMailboxRepositry.VerifyUser(username, password);

                if (RemoteUser != null)
                {
                    Send("235 OK, GO AHEAD");
                }
                else
                {
                    Send("500 Bad Username or Password");
                    _stream.Close();
                    return;
                }
            }

            do
            {
                ReadMailMessage();

                var reset = ReadResponse();
                if (reset.ToUpper() == "RSET")
                {
                    Send("250 OK");
                }
            }
            while (response.ToUpper() == "RSET");

            _stream.Close();
            
        }
    }
}
