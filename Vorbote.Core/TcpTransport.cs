namespace Vorbote
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets; 

    public class TcpTransport : ITransport, IDisposable
    {
        protected NetworkStream _networkStream;
        protected StreamReader _streamReader;
        protected StreamWriter _streamWriter;

        public TcpTransport(NetworkStream networkStream)
        {
            _networkStream = networkStream;
        }

        virtual protected void Initialize()
        {
            _streamReader = new StreamReader(_networkStream);
            _streamWriter = new StreamWriter(_networkStream);
        }

        public void Close()
        {
            _networkStream.Close();
        }

        virtual public void Dispose()
        {
            if(_streamReader != null)
            {
                _streamReader.Dispose();
                _streamReader = null;
            }

            if (_streamWriter != null)
            {
                _streamWriter.Dispose();
                _streamWriter = null;
            }

            if (_networkStream != null)
            {
                _networkStream.Dispose();
                _networkStream = null;
            }
        }

        public string Read()
        {
            var response = _streamReader.ReadLine();
            return response;
        }

        public void Send(params string[] messages)
        {
            foreach(var message in messages)
            {
                Send(message);
            }
        }

        private void Send(string message)
        {
            _streamWriter.WriteLine(message);
        }

        public void Send(SmtpStatusCode statusCode, string message)
        {
            var data = string.Format("{0} {1}", statusCode.ToString(), message);
            Send(data);
        }

        public void SendFormat(string messageFormat, params object[] args)
        {
            var data = string.Format(messageFormat,  args);
            Send(data);
        }
    }
}
