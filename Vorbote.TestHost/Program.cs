namespace Vorbote.TestHost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Vorbote.Accounts;

    class Program
    {
        static  void Main(string[] args)
        {
            Server server = new Server(true);

            server.Init();
            server.Start();

            Console.ReadLine();
        }
    }
}
