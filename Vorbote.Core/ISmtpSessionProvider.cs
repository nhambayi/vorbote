using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Vorbote
{
    public interface ISmtpSessionProvider 
    {
        Task<IResult> RunAsync(SmtpSessionContext context, CancellationToken cancellationToken = new CancellationToken());
    }
}
