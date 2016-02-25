namespace Vorbote
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class SmtpSessionContext
    {
        public string ServerName { get; set; }

        public ITransport Transport { get; set; }

        public IAuthProvider AuthProvider { get; set; }

        public IMessageSenderValidator SenderValidator { get; set; }

        public IMessageRecipientValidator RecipientValidator { get; set; }

        public IMessageStore MessageStore { get; set; }
    }
}
