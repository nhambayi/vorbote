using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vorbote
{
    public class RecipientValidationProvider
    {
        public async Task<IResult> RunAsync(SmtpSessionContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var transport = context.Transport;

            var recipientMessage = transport.Read();
            List<string> recipients = new List<string>();

            do
            {
                if (!recipientMessage.StartsWith("RCPT TO:"))
                {
                    var errorResult = new RecipientValidationResult
                    {
                        StatusCode = SmtpStatusCode.UNKNOWN_COMMAND,
                        StatusReason = "UNKNOW COMMAND"
                    };
                    return errorResult;
                }
                else
                {
                    var recipient = recipientMessage.Replace("RCPT TO:", string.Empty).Trim();
                    var validationResult = context.RecipientValidator.ValidateRecipient(recipient);

                    if (validationResult)
                    {
                        recipients.Add(recipient);
                        transport.SendFormat("250 {0} OK", recipients);
                    }
                }
                recipientMessage = transport.Read();

            } while (recipientMessage.Trim() != "DATA");

            var result = new RecipientValidationResult
            {
                StatusCode = SmtpStatusCode.OK,
                StatusReason = "ACCEPTED",
                Recipients = recipients
            };
            return result;
        }
    }
}
