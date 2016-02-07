using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vorbote
{
    public class SenderValidationProvider 
    {
        public async Task<IResult> RunAsync(SmtpSessionContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var transport = context.Transport;
            var senderMessage = transport.Read();

            if (!senderMessage.StartsWith("MAIL FROM:"))
            {
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
