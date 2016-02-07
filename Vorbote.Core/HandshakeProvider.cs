using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vorbote
{
    public class HandshakeProvider
    {
        public async Task<IResult> RunAsync(SmtpSessionContext context, 
            CancellationToken cancellationToken = new CancellationToken())
        {
            var transport = context.Transport;
            transport.SendFormat("220 {0} SMTP server ready.", context.ServerName);

            string response = transport.Read();
            if (!response.StartsWith("HELO") && !response.StartsWith("EHLO"))
            {
                var errorResult = new HandshakeResult
                {
                    StatusCode = 500,
                    StatusReason = "Unknow Command"
                };
                return errorResult;
            }
            string client = response.Replace("HELO", string.Empty).Replace("EHLO", string.Empty).Trim();
            transport.Send("250-localhost");

            var result = new HandshakeResult
            {
                StatusCode = 250,
                RemoteClient = client
            };
            return result;
        }
    }
}
