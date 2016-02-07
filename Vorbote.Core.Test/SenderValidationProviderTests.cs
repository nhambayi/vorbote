using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Vorbote.Core.Tests
{
    [TestClass]
    public class SenderValidationProviderTests
    {
        [TestMethod]
        public void SenderValidationSuccess()
        {
            var provider = new SenderValidationProvider();

            var transportMoq = new Mock<ITransport>(MockBehavior.Loose);
            var context = new SmtpSessionContext()
            {
                Transport = transportMoq.Object
            };
            transportMoq.Setup(m => m.Read()).Returns("MAIL FROM:<batman@gotham.com");

            var result = provider.RunAsync(context).Result as SenderValidationResult;

            Assert.IsNotNull(result, "Response is of the incorrect type");
            Assert.AreEqual(200, result.StatusCode);
        }

        [TestMethod]
        public void SenderValidationBadRequest()
        {
            var provider = new SenderValidationProvider();

            var transportMoq = new Mock<ITransport>(MockBehavior.Loose);
            var context = new SmtpSessionContext()
            {
                Transport = transportMoq.Object
            };
            transportMoq.Setup(m => m.Read()).Returns("BAD COMMAND");

            var result = provider.RunAsync(context).Result as SenderValidationResult;

            Assert.IsNotNull(result, "Response is of the incorrect type");
            Assert.AreEqual(500, result.StatusCode);
        }
    }
}
