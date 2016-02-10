using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote
{
    public interface ITransport
    {
        string Read();
        void Send(params string[]  message);
        void SendFormat(string messageFormat, params object[] args);
        void Send(SmtpStatusCode statusCode, string message);
        void Close();

    }
}
