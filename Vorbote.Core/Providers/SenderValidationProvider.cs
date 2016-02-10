using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vorbote.Providers
{

    public class SenderValidationProvider : ISmtpSessionProvider
    {
        public async Task<IResult> RunAsync(SmtpSessionContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var transport = context.Transport;
            var senderMessage = transport.Read();

            if (!senderMessage.StartsWith("MAIL FROM:"))
            {
                transport.Send(SmtpStatusCode.UNKNOWN_COMMAND, "UNKNOW COMMAND");
                var errorResult = new SenderValidationResult
                {
                    StatusCode = SmtpStatusCode.UNKNOWN_COMMAND,
                    StatusReason = "Unknown Command"
                };
                return errorResult;
            }
            else
            {
                var sender = senderMessage.Replace("MAIL FROM:", string.Empty).Trim();
                var validationResult = context.SenderValidator.IsAuthorizedSender(sender);

                if (validationResult)
                {
                    transport.Send(SmtpStatusCode.OK, "GO AHEAD");
                    var result = new SenderValidationResult
                    {
                        StatusCode = SmtpStatusCode.OK,
                        StatusReason = "Sender Accepted",
                        Sender = sender
                    };
                    return result;
                }
                else
                {
                    transport.Send(SmtpStatusCode.MAILBOX_NOT_FOUND, "UNAUTHORIZED SENDER");
                    var result = new SenderValidationResult
                    {
                        StatusCode = SmtpStatusCode.MAILBOX_NOT_FOUND,
                        StatusReason = "Sender Not Authorized",
                        Sender = sender
                    };
                    return result;
                }
            }
        }
    }
}
