using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vorbote.Models;

namespace Vorbote
{
    public interface IMessageStore
    {
        void SaveMessage(IMailMessage message);
    }
}
