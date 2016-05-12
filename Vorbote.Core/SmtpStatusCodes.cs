using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote
{
    public enum SmtpStatusCode
    {
        SYSTEM_STATUS = 211,
        HELP_MESSAGE = 214,
        SERVER_READY = 220,
        SERVER_CLOSING_CHANNEL = 221,
        OK = 250,
        USER_NOT_LOCAL = 251,
        START_MAIL_INPUT = 354,
        SERVICE_NOT_AVAILABLE = 421,
        MAILBOX_NOT_AVAILABLE = 450,
        LOCAL_PROCESSING_ERROR = 451,
        INSUFFICIENT_STORAGE = 452,
        UNKNOWN_COMMAND = 500,
        INVALID_ARGUMENTS = 501,
        COMMAND_NOT_IMPLEMENTED = 502,
        BAD_COMMAND_SEQUENCE = 503,
        COMMAND_PARAMETER_NOT_IMPLEMENTED = 504,
        MAILBOX_NOT_FOUND = 550,
        FORWARD_MESSAGE = 551,
        MAILBOX_FUll = 552,
        MAILBOX_FULL_NOT_ALLOWED = 553,
        TRANSACTION_FAILED = 554
    }
}
