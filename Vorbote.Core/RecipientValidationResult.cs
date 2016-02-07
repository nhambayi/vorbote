using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote
{
    public class RecipientValidationResult : IResult
    {
        public SmtpStatusCode StatusCode { get; set; }

        public string StatusReason { get; set; }

        public IEnumerable<string> Recipients { get; set; }
    }
}
