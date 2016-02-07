using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote
{
    public class SenderValidationProvider 
    {
        public async Task<IResult> RunAsync(SmtpSessionContext context)
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
                var validationResult = context.SenderStore.IsAuthorizedSender(sender);
                if(validationResult)
                {
                    var result = new SenderValidationResult
                    {
                        StatusCode = 200,
                        StatusReason = "Sender Accepted",
                        Sender = sender
                    };
                }
                else
                {
                    var result = new SenderValidationResult
                    {
                        StatusCode = 400,
                        StatusReason = "Sender Not Authorized",
                        Sender = sender
                    };
                }

                return result;
            }
        }
    }
}
