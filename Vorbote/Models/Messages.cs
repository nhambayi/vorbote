namespace Vorbote.Models
{
    using System;

    public class MailMessage : IMailMessage
    {
        public string To { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public DateTime Sent { get; set; }
    }
}
