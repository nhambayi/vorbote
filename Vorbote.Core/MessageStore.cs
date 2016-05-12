namespace Vorbote
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    public abstract class MessageStore : IDisposable, IMessageStore
    {
        protected Stream _messageStream;
        protected StreamWriter _messageStreamWritter;
        private readonly SmtpSessionContext _sessionContext;

        public MessageStore(SmtpSessionContext sessionContext)
        {
            _sessionContext = sessionContext;
        }

        public virtual async Task WriteAsync(string data)
        {
            await  _messageStreamWritter.WriteAsync(data);
        }

        public virtual async Task WriteLineAsync(string data)
        {
            await _messageStreamWritter.WriteLineAsync(data);
        }

        public virtual void Dispose()
        {
            if (_messageStream != null)
            {
                _messageStream.Dispose();
                _messageStream = null;
            }
            if (_messageStreamWritter != null)
            {
                _messageStreamWritter.Dispose();
                _messageStreamWritter = null;
            }
        }
    }
}
