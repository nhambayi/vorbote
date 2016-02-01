namespace Vorbote.Mailboxes.SqlServer
{
    public class MailboxMessages
    {
        public int MessageId { get; set; }

        public int MailboxMessageID { get; set; }

        public int MailboxId { get; set; }

        public string RawHeader { get; set; }

        public string Body { get; set; }

        public string MessageUrl { get; set; }
    }
}
