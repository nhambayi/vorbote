using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote
{
    interface IAllowedSenderStore
    {
        bool IsAuthorizedSender(string sender);
    }
}
