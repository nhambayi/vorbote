using System;

namespace Vorbote.Mailboxes.SqlServer
{
    public class MailboxMessage
    {
        public string MessageId { get; set; }

        public int MailboxMessageID { get; set; }

        public int MailboxId { get; set; }

        public string RawHeader { get; set; }

        public string Body { get; set; }

        public string MessageUrl { get; set; }

        public DateTime Recieved { get; set; }

        public bool IsRead { get; set; }
    }
}
