namespace Vorbote.Mailboxes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IMailboxRepositry
    {
        void AddNewMailbox(IMailbox mailbox);

        IMailbox GetMailbox(string username);

    }
}
