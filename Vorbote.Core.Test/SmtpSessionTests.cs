using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Vorbote.Core.Tests
{
    [TestClass]
    public class SmtpSessionTests
    {
        private Mock<ISmtpSessionProvider> _authProviderMock;
        private Mock<ISmtpSessionProvider> _messageHandlerMock;
        private Mock<ISmtpSessionProvider> _recipientValidationProviderMock;
        private Mock<ISmtpSessionProvider> _senderValidationProviderMock;
        private Mock<ISmtpSessionProvider> _handshakeProviderMock;
        private SmtpSession _session;

        [TestInitialize]
        public void Setup()
        {
            SmtpSessionContext context = new SmtpSessionContext();
            Mock<ITransport> transportMock = new Mock<ITransport>(MockBehavior.Loose);

            _handshakeProviderMock = CreateProvider(context, new Result { StatusCode = SmtpStatusCode.OK });
            _senderValidationProviderMock = CreateProvider(context, new Result { StatusCode = SmtpStatusCode.OK });
            _recipientValidationProviderMock = CreateProvider(context, new Result { StatusCode = SmtpStatusCode.OK });
            _messageHandlerMock = CreateProvider(context, new Result { StatusCode = SmtpStatusCode.OK });
            _authProviderMock = CreateProvider(context, new Result { StatusCode = SmtpStatusCode.OK });


            _session = new SmtpSession(transportMock.Object)
            {
                HandshakeProvider = _handshakeProviderMock.Object,
                SenderValidationProvider = _senderValidationProviderMock.Object,
                RecipientValidationProvider = _recipientValidationProviderMock.Object,
                MessageReaderProvider = _messageHandlerMock.Object,
                AuthProvider = _authProviderMock.Object
            };

            _session.StartSession().Wait();
        }

        [TestMethod]
        public void TransportIsNotNull()
        {
            Assert.IsNotNull(_session.Transport);
        }


        [TestMethod]
        public void HandShakeProviderWasExecuted()
        {
            _handshakeProviderMock.VerifyAll();
        }

        [TestMethod]
        public void AuthProviderWasExecuted()
        {
            _authProviderMock.VerifyAll();
        }

        [TestMethod]
        public void SenderVerificationProviderWasExecuted()
        {
            _senderValidationProviderMock.VerifyAll();
        }

        [TestMethod]
        public void RecipientVerificationProviderWasExecuted()
        {
            _recipientValidationProviderMock.VerifyAll();
        }

        [TestMethod]
        public void MessageHandlerWasExecute()
        {
            _messageHandlerMock.VerifyAll(); ;
        }

        private static Mock<ISmtpSessionProvider> CreateProvider(SmtpSessionContext ItAny, IResult result)
        {
            Mock<ISmtpSessionProvider> providerMock = new Mock<ISmtpSessionProvider>();
            providerMock.Setup(s => s.RunAsync(It.IsAny<SmtpSessionContext>(), It.IsAny<CancellationToken>()) )
                .Returns(Task.FromResult<IResult>(result))
                .Verifiable("Was not called");

            return providerMock;
        }
    }
}
