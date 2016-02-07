using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vorbote
{
    public class MailMessageReadProvider
    {
        public async Task<IResult> RunAsync(SmtpSessionContext context, 
            CancellationToken cancellationToken = new CancellationToken())
        {
            var transport = context.Transport;
            int counter = 0;
            StringBuilder message = new StringBuilder();
            string response;

            while ((response = transport.Read().Trim()) != ".")
            {

                message.AppendLine(response);
                counter++;

                if (counter == 1000000)
                {
                    var errorResult = new MessageProcessingResult
                    {
                        StatusCode = SmtpStatusCode.UNKNOWN_COMMAND,
                        StatusReason = "Message size exceeds limit"
                    };

                    return errorResult;
                }
            }

            transport.Send("250 OK");
            var result = new MessageProcessingResult
            {
                StatusCode = SmtpStatusCode.OK,
                StatusReason = "Message recieved"
            };
            return result;
        }
    }
}
