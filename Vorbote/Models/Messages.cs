﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote.Models
{
    public class MailMessage : IMailMessage
    {
        public string To { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public DateTime Sent { get; set; }
    }
}
