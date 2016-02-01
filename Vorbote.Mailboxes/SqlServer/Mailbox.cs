using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote.Mailboxes.SqlServer
{
    public class Mailbox
    {
        public int MailboxId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Username, MailboxId);
        }
    }
}
