﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vorbote
{
    public class PlainTextLoginProvider
    {
        public async Task<IResult> RunAsync(SmtpSessionContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var transport = context.Transport;
            transport.Send("250 AUTH LOGIN PLAIN CRAM-MD5");
            var response = transport.Read();

            string username = null;
            if (response.ToUpper() == "AUTH LOGIN")
            {
                transport.Send("334 VXNlcm5hbWU6");

                var encodedUsername = transport.Read();
                username = encodedUsername.Base64Decode();

                transport.Send("334 UGFzc3dvcmQ6");
                var passwordResponse = transport.Read();
                var password = passwordResponse.Base64Decode();

                var isValid = context.AuthProvider.AuthorizeUser(username, password);

                if (isValid)
                {
                    var result = new UserAuthenticationResult
                    {
                        StatusCode =  SmtpStatusCode.OK,
                        StatusReason = "User Authenticated",
                        Username = username,
                        MailBox = null
                    };
                    return result;
                }
                else
                {
                    var errorResult = new UserAuthenticationResult
                    {
                        StatusCode = SmtpStatusCode.MAILBOX_NOT_FOUND,
                        StatusReason = "Bad Username or Password",
                        Username = username,
                        MailBox = null
                    };
                    return errorResult;
                }
            }
            else
            {
                var errorResult = new UserAuthenticationResult
                {
                    StatusCode = SmtpStatusCode.UNKNOWN_COMMAND,
                    StatusReason = "Unknow Command",
                    Username = null,
                    MailBox = null
                };

                return errorResult;
            }
        }
    }
}
