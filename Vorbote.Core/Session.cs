using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vorbote
{
    public class SmtpSession : ISession
    {
        private CancellationTokenSource _tokenSource;
        private SmtpSessionContext _sessionContext;

        public SmtpSession(ITransport transport)
        {
            
        }

        public async Task StartSession(CancellationToken cancellationToken = new CancellationToken())
        {
            await ProcessSession();
        }

        virtual protected async Task ProcessSession()
        {
            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            var handshakeResult = await HandshakeProvider.RunAsync(_sessionContext, token);

            if (AuthProvider != null)
            {
                var authresult = await AuthProvider.RunAsync(_sessionContext, token);
            }

            var senderValidationResult = await SenderValidationProvider.RunAsync(_sessionContext, token);
            var recipientValidationResult = await RecipientValidationProvider.RunAsync(_sessionContext, token);
            var messageReaderResult = await MessageReaderProvider.RunAsync(_sessionContext, token);
        }

        public SmtpSessionContext SessionCOntext { get; private set; }

        public ISmtpSessionProvider HandshakeProvider { get; set; }
        public ISmtpSessionProvider AuthProvider { get; set; }
        public ISmtpSessionProvider SenderValidationProvider { get; set; }
        public ISmtpSessionProvider RecipientValidationProvider { get; set; }
        public ISmtpSessionProvider MessageReaderProvider { get; set; }

    }
}
