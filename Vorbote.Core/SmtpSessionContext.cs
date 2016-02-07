using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote
{
    public class SmtpSessionContext
    {
        public string ServerName { get; set; }

        public ITransport Transport { get; set; }

        public IAuthProvider AuthProvider { get; set; }

        public IMessageSenderValidator SenderValidator { get; set; }

        public IMessageRecipientValidator RecipientValidator { get; set; }
    }
}
