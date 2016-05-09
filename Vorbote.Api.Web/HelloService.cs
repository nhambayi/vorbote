using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vorbote.Api.Web
{
    public class HelloService : Service
    {
        public object Any(Hello request)
        {
            return new HelloResponse { Result = "Hello, " + request.Name };
        }
    }
}
