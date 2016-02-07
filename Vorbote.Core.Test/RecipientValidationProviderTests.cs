using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Vorbote.Core.Tests
{
    [TestClass]
    public class RecipientValidationProviderTests
    {
        private int _counter;
        private Mock<ITransport> _transportMock;
        private List<string> _responses;
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void RecipientValidationSucessTest()
        {
            var provider = new RecipientValidationProvider();
            var transportMoq = new Mock<ITransport>(MockBehavior.Loose);
            const string recipientAddress = "robin.hood@sherwoodforest.co.uk";
            SetResponses(string.Format("RCPT TO: {0}", recipientAddress), "DATA");

            Mock<IMessageRecipientValidator> validatorMock = new Mock<IMessageRecipientValidator>();
            validatorMock.Setup(s => s.ValidateRecipient(It.IsAny<string>())).Returns(true);

            var context = new SmtpSessionContext()
            {
                Transport = _transportMock.Object,
                RecipientValidator = validatorMock.Object
            };

            var result = provider.RunAsync(context).Result as RecipientValidationResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(SmtpStatusCode.OK, result.StatusCode);
            Assert.IsNotNull(result.Recipients);
            List<string> reciientList = new List<string>(result.Recipients);
            Assert.IsTrue(reciientList.Count == 1);
            Assert.AreEqual(recipientAddress ,reciientList[0]);
        }

        [TestMethod]
        public void RecipientValidationBadCommandTest()
        {
            var provider = new RecipientValidationProvider();
            var transportMoq = new Mock<ITransport>(MockBehavior.Loose);
            const string recipientAddress = "robin.hood@sherwoodforest.co.uk";
            SetResponses(string.Format("MAIL TO: {0}", recipientAddress), "DATA");

            var context = new SmtpSessionContext()
            {
                Transport = _transportMock.Object
            };


            var result = provider.RunAsync(context).Result as RecipientValidationResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(SmtpStatusCode.UNKNOWN_COMMAND, result.StatusCode);
            Assert.IsNull(result.Recipients);
        }

        private void SetResponses(params string[] args)
        {
            _responses = new List<string>(args);

            _transportMock = new Mock<ITransport>();
            _transportMock.Setup(s => s.Read()).Returns(() =>
            {
                var message = _responses[_counter];
                _counter++;
                return message;
            });

            _counter = 0;
        }
    }
}
