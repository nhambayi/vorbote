using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vorbote
{
    public class SmtpSession : ISession, IDisposable
    {
        private CancellationTokenSource _tokenSource;
        private readonly SmtpSessionContext _sessionContext;

        public event EventHandler<HandshakeCompleteEventArgs> ClientHandshakeCompleted;
        public event EventHandler<HandshakeCompleteEventArgs> ClientHandshakeStarted;
        public event EventHandler<HandshakeCompleteEventArgs> UserAuthenticationStarted;
        public event EventHandler<HandshakeCompleteEventArgs> UserAuthenticationCompleted;
        public event EventHandler<HandshakeCompleteEventArgs> SenderValidationStarted;
        public event EventHandler<HandshakeCompleteEventArgs> SenderValidationCompleted;
        public event EventHandler<HandshakeCompleteEventArgs> RecipientValidationStarted;
        public event EventHandler<HandshakeCompleteEventArgs> RecipientValidationCompleted;

        public SmtpSession(ITransport transport)
        {
            Transport = transport;
        }

        public SmtpSessionContext SessionCOntext { get; private set; }
        public ISmtpSessionProvider HandshakeProvider { get; set; }
        public ISmtpSessionProvider AuthProvider { get; set; }
        public ISmtpSessionProvider SenderValidationProvider { get; set; }
        public ISmtpSessionProvider RecipientValidationProvider { get; set; }
        public ISmtpSessionProvider MessageReaderProvider { get; set; }
        public ITransport Transport { get; set; }

        public async Task StartSession(CancellationToken cancellationToken = new CancellationToken())
        {
            await ProcessSession();
        }

        virtual protected async Task ProcessSession()
        {
            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            var handshakeResult = await HandshakeProvider.RunAsync(_sessionContext, token);
            SendReponse(handshakeResult.StatusReason, handshakeResult.StatusCode);

            if (AuthProvider != null)
            {
                var authresult = await AuthProvider.RunAsync(_sessionContext, token);
                SendReponse(authresult.StatusReason, authresult.StatusCode);
            }

            var senderValidationResult = await SenderValidationProvider.RunAsync(_sessionContext, token);
            SendReponse(senderValidationResult.StatusReason, senderValidationResult.StatusCode);

            var recipientValidationResult = await RecipientValidationProvider.RunAsync(_sessionContext, token);
            SendReponse(recipientValidationResult.StatusReason, recipientValidationResult.StatusCode);

            var messageReaderResult = await MessageReaderProvider.RunAsync(_sessionContext, token);
            SendReponse(messageReaderResult.StatusReason, messageReaderResult.StatusCode);
        }

        public void Dispose()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Dispose();
                _tokenSource = null;
            }
        }

        #region OnClientHandshakeCompleted
        /// <summary>
        /// Triggers the ClientHandshakeCompleted event.
        /// </summary>
        public virtual void OnClientHandshakeCompleted(HandshakeCompleteEventArgs ea)
        {
            EventHandler<HandshakeCompleteEventArgs> handler = ClientHandshakeCompleted;
            if (handler != null)
                handler(null/*this*/, ea);
        }

        #region OnClientHandshakeStarted
        /// <summary>
        /// Triggers the ClientHandshakeStarted event.
        /// </summary>
        public virtual void OnClientHandshakeStarted(HandshakeCompleteEventArgs ea)
        {
            EventHandler<HandshakeCompleteEventArgs> handler = ClientHandshakeStarted;
            if (handler != null)
                handler(null/*this*/, ea);
        }
        #endregion
        #endregion
        private void SendReponse(string message, SmtpStatusCode statusCode)
        {
            Transport.Send("{0} {1}", statusCode.ToString(), message);
        }
    }
}
