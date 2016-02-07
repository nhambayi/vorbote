using Moq;
using System;
using Vorbote;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vorbote.Tests
{
    [TestClass()]
    public class PlainTextLoginProviderTests
    {
        private Mock<IAuthProvider> _authProvider;
        private int _counter;
        private Mock<ITransport> _transportMock;
        private List<string> _responses;

        [TestMethod()]
        public void CorrectusernameAndPassword()
        {
            var testUsername = "testuser";
            var testPassword = "password";

            SetResponses(testUsername, testPassword);

            ResetCounter();
            SetupTransport();
            SetupAuthProvider(true);

            SmtpSessionContext context = new SmtpSessionContext
            {
                AuthProvider = _authProvider.Object,
                Transport = _transportMock.Object
            };

            PlainTextLoginProvider provider = new PlainTextLoginProvider();
            var result = provider.RunAsync(context).Result;

            var authresult = result as UserAuthenticationResult;
            Assert.IsNotNull(authresult);
            Assert.AreEqual(200, authresult.StatusCode);
            Assert.AreEqual(testUsername, authresult.Username);
        }

        [TestMethod()]
        public void BadUsernameOrPassword()
        {
            var testUsername = "testuser";
            var testPassword = "password";

            SetResponses(testUsername, testPassword);

            ResetCounter();
            SetupTransport();
            SetupAuthProvider(false);

            SmtpSessionContext context = new SmtpSessionContext
            {
                AuthProvider = _authProvider.Object,
                Transport = _transportMock.Object
            };

            PlainTextLoginProvider provider = new PlainTextLoginProvider();
            var result = provider.RunAsync(context).Result;

            var authresult = result as UserAuthenticationResult;
            Assert.IsNotNull(authresult);
            Assert.AreEqual(400, authresult.StatusCode);
            Assert.AreEqual(testUsername, authresult.Username);
        }

        [TestMethod]
        public void InvalidAuthHandshake()
        {
            _responses = new List<string>();
            _responses.Add("Bad Response");

            ResetCounter();
            SetupTransport();

            SmtpSessionContext context = new SmtpSessionContext
            {
                Transport = _transportMock.Object
            };

            PlainTextLoginProvider provider = new PlainTextLoginProvider();
            var result = provider.RunAsync(context).Result;

            var authresult = result as UserAuthenticationResult;
            Assert.IsNotNull(authresult);
            Assert.AreEqual(500, authresult.StatusCode);
        }

        private void SetupAuthProvider(bool authproviderReturn)
        {
            _authProvider = new Mock<IAuthProvider>();
            _authProvider.Setup(s =>
            s.AuthorizeUser(It.IsAny<string>(), It.IsAny<string>())).Returns(authproviderReturn);
        }

        private void SetupTransport()
        {
            _transportMock = new Mock<ITransport>();
            _transportMock.Setup(s => s.Read()).Returns(() =>
            {
                var message = _responses[_counter];
                _counter++;
                return message;
            });
        }

        private void ResetCounter()
        {
            _counter = 0;
        }

        private List<string> SetResponses(string testUsername, string testPassword)
        {
            _responses = new List<string>();
            _responses.Add("AUTH LOGIN");

            _responses.Add(testUsername.Base64Encode());
            _responses.Add(testPassword.Base64Encode());
            return _responses;
        }
    }
}