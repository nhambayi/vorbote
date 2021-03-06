﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote.Providers
{
    public class UserAuthenticationResult : IResult
    {
        public SmtpStatusCode StatusCode { get; set; }

        public string StatusReason { get; set; }

        public string Username { get; set; }

        public bool Authenticated { get; set; }

        public object MailBox { get; set; }
    }
}
