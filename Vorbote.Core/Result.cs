using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vorbote
{
    public class Result : IResult
    {
        public SmtpStatusCode StatusCode { get; set; }
        public string StatusReason { get; set; }
    }
}
