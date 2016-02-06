using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vorbote
{
    public class Result : IResult
    {
        public Result()
        {

        }
        public int StatusCode { get; set; }
        public string StatusReason { get; set; }
    }
}
