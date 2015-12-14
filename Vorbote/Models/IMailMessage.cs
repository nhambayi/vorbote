namespace Vorbote.Models
{
    using System;

    public interface IMailMessage
    {
        string Body { get; set; }
        string From { get; set; }
        DateTime Sent { get; set; }
        string Subject { get; set; }
        string To { get; set; }
    }
}