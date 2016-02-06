using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote
{
    public class SenderValidationProvider 
    {
        public IResult RunAsync(SmtpSessionContext context)
        {
            var transport = context.Transport;

            var senderMessage = transport.Read();

            if (!senderMessage.StartsWith("MAIL FROM:"))
            {
                var errorResult = new SenderValidationResult
                {
                    StatusCode = 500,
                    StatusReason = "Unknown Command"
                };

                return errorResult;
            }
            else
            {
                var sender = senderMessage.Replace("MAIL FROM:", string.Empty).Trim();
                var result = new SenderValidationResult
                {
                    StatusCode = 200,
                    StatusReason = "Sender Accepted",
                    Sender = sender
                };
                return result;
            }
        }
    }
}
