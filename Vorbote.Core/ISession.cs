using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Vorbote
{
    public interface ISession
    {
        Task StartSession(CancellationToken cancellationToken = new CancellationToken());
        SmtpSessionContext SessionCOntext { get; }
        ISmtpSessionProvider HandshakeProvider { get; set; }
        ISmtpSessionProvider AuthProvider { get; set; }
        ISmtpSessionProvider SenderValidationProvider { get; set; }
        ISmtpSessionProvider RecipientValidationProvider { get; set; }
        ISmtpSessionProvider MessageReaderProvider { get; set; }
    }
}
