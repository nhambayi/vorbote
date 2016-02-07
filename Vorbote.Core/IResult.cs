using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote
{
    public interface IResult
    {
        int StatusCode { get; set; }
        string StatusReason { get; set; }
    }
}
