using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote
{
    public interface IMessageQueue
    {
        void QueueMessage(string id, string account, string headers);
    }
}
