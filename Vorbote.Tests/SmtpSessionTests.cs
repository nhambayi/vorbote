using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vorbote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Vorbote.Tests
{
    [TestClass()]
    public class SmtpSessionTests
    {
        [TestMethod()]
        public void StartSessionTest()
        {
            MemoryStream stream = new MemoryStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writter = new StreamWriter(stream);

            SmtpSession session = new SmtpSession(reader, writter);
            //session.StartSession();

            Assert.Fail();
        }
    }
}