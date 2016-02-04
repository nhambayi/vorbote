using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote
{
    public class SmtpConstants
    {
        public const string GO_AHEAD = "235 OK, GO AHEAD";
        public const string UNKNOWN_COMMAND = "500 UNKNOWN COMMAND";
        public const string READY = "220 {0} SMTP SERVER READY.";
        public const string HELO = "HELO";
        public const string EHLO = "EHLO";
        public const string AUTH_LOGIN_PLAIN = "250 AUTH LOGIN PLAIN CRAM-MD5";

    }
}
