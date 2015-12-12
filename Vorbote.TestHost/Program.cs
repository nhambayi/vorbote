using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vorbote.Accounts;

namespace Vorbote.TestHost
{
    class Program
    {
        static  void Main(string[] args)
        {
            DocumentDbMailBoxRepositry db = new DocumentDbMailBoxRepositry();
            db.Initialize();

            var mail = db.GetMailBox("noahhambayi");
            Server server = new Server(true);

            server.Init();
            server.Start();

            Console.ReadLine();
        }
    }
}
