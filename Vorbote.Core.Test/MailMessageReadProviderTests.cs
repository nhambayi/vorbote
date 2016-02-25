using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vorbote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Vorbote.Providers;

namespace Vorbote.Tests
{
    [TestClass()]
    public class MailMessageReadProviderTests
    {
        private int _counter;
        private List<string> _responses;
        private Mock<ITransport> _transportMock;

        [TestMethod()]
        public void ReadMessageTest()
        {
            var responses = new string[]{
                "From: \"Bob Example\" < bob@example.org >",
                "To: \"Alice Example\" < alice@example.com >",
                "Cc: theboss @example.com",
                "Date: Tue, 15 January 2008 16:02:43 - 0500",
                "Subject: Test message",
                "",
                "Hello Alice.",
                "This is a test message with 5 header fields and 4 lines in the message body.",
                "Your friend,",
                "Bob",
                "."};

            SetResponses(responses);
            
            var provider = new MailMessageReadProvider();

            var context = new SmtpSessionContext()
            {
                Transport = _transportMock.Object
            };

            var result = provider.RunAsync(context).Result as MessageProcessingResult;

            Assert.IsNotNull(result);
        }

        private void SetResponses(params string[] args)
        {
            _responses = new List<string>(args);
            _transportMock = new Mock<ITransport>();

            _transportMock.Setup(s => s.Read()).Returns(() =>
            {
                return _responses[_counter];
            })
            .Callback(() => 
            {
                _counter++;
            });

            _counter = 0;
        }
    }
}