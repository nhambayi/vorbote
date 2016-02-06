using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote
{
    public class PlainTextLoginProvider
    {
        public IResult RunAsync(SmtpSessionContext context)
        {
            var transport = context.Transport;

            transport.Send("250 AUTH LOGIN PLAIN CRAM-MD5");
            var response = transport.Read();

            string username = null;
            if (response.ToUpper() == "AUTH LOGIN")
            {
                transport.Send("334 VXNlcm5hbWU6");

                var encodedUsername = transport.Read();
                username = Base64Decode(encodedUsername);

                transport.Send("334 UGFzc3dvcmQ6");
                var passwordResponse = transport.Read();
                var password = Base64Decode(passwordResponse);

                var isValid = context.AuthProvider.AuthorizeUser(username, password);

                if (isValid)
                {
                    var result = new UserAuthenticationResult
                    {
                        StatusCode = 200,
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
                        StatusCode = 400,
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
                    StatusCode = 400,
                    StatusReason = "Unknow Command",
                    Username = null,
                    MailBox = null
                };

                return errorResult;
            }
        }

        private static string Base64Decode(string encodedUsername)
        {
            string username;
            var data = Convert.FromBase64String(encodedUsername);
            username = Encoding.UTF8.GetString(data);
            return username;
        }
    }
}
