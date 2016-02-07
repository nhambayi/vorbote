using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote
{
    public class RecipientValidationProvider
    {
        public IResult RunAsync(SmtpSessionContext context)
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
                        StatusCode = 500,
                        StatusReason = "Unknow Command"
                    };
                    return errorResult;
                }
                else
                {
                    var recipient = recipientMessage.Replace("MAIL FROM:", string.Empty).Trim();
                    recipients.Add(recipient);
                    transport.SendFormat("250 {0} :-)", recipients);
                }
                recipientMessage = transport.Read();

            } while (recipientMessage.Trim() != "DATA");

            var result = new RecipientValidationResult
            {
                StatusCode = 200,
                StatusReason = "Recipients Accepted",
                Recipients = recipients
            };
            return result;
        }
    }
}
