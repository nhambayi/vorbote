using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vorbote
{
    [Serializable]
    public class EmailReceivedMessage
    {

        public string Account { get; set; }

        public EmailHeader[] Headers { get; set; }

        public string RawHeaders { get; set; }

        public string MessageKey { get; set; }
    }
}
