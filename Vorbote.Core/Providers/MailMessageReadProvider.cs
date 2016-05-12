using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vorbote.Providers
{
    public class MailMessageReadProvider : ISmtpSessionProvider
    {
        public IResult Run(SmtpSessionContext context)
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
                    transport.Send(SmtpStatusCode.INSUFFICIENT_STORAGE, "MESSAGE TO LARGE");
                    var errorResult = new MessageProcessingResult
                    {
                        StatusCode = SmtpStatusCode.LOCAL_PROCESSING_ERROR,
                        StatusReason = "Message size exceeds limit"
                    };

                    return errorResult;
                }
            }

            transport.Send(SmtpStatusCode.OK, "OK");

            var result = new MessageProcessingResult
            {
                StatusCode = SmtpStatusCode.OK,
                StatusReason = "Message recieved"
            };
            return result;
        }

        public async Task<IResult> RunAsync(SmtpSessionContext context, 
            CancellationToken cancellationToken = new CancellationToken())
        {
            return Run(context);
        }
    }
}
