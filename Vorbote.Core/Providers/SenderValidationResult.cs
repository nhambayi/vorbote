using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote.Providers
{
    public class SenderValidationResult : IResult
    {
        public SmtpStatusCode StatusCode { get; set; }

        public string StatusReason { get; set; }

        public string Sender { get; set; }
    }
}
