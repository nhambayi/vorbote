﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vorbote.Providers
{
    public class HandshakeProvider : ISmtpSessionProvider
    {
        public IResult Run(SmtpSessionContext context)
        {
            var transport = context.Transport;
            transport.SendFormat("220 {0} SMTP server ready.", context.ServerName);

            string response = transport.Read();
            if (!response.StartsWith("HELO") && !response.StartsWith("EHLO"))
            {
                var errorResult = new HandshakeResult
                {
                    StatusCode = SmtpStatusCode.UNKNOWN_COMMAND,
                    StatusReason = "Unknow Command"
                };
                return errorResult;
            }
            string client = response.Replace("HELO", string.Empty).Replace("EHLO", string.Empty).Trim();
            transport.Send("250-localhost");

            var result = new HandshakeResult
            {
                StatusCode = SmtpStatusCode.OK,
                RemoteClient = client
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
