﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Vorbote.Core.Test
{
    [TestClass]
    public class HandshakeTests
    {
        [TestMethod]
        public void BasicHandshake()
        {
            var provider = new HandshakeProvider();
            
            var transportMoq = new Mock<ITransport>(MockBehavior.Loose);
            var context = new SmtpSessionContext()
            {
                ServerName = "localhost",
                Transport = transportMoq.Object

            };
            transportMoq.Setup(m => m.Read()).Returns("HELO");

            var result = provider.RunAsync(context);

            Assert.AreEqual(250, result.StatusCode);
        }

        [TestMethod]
        public void BasicHandshakeEHLOResponse()
        {
            var provider = new HandshakeProvider();

            var transportMoq = new Mock<ITransport>(MockBehavior.Loose);
            var context = new SmtpSessionContext()
            {
                ServerName = "localhost",
                Transport = transportMoq.Object

            };
            transportMoq.Setup(m => m.Read()).Returns("EHLO");

            var result = provider.RunAsync(context);

            Assert.AreEqual(250, result.StatusCode);
        }

        [TestMethod]
        public void BasicHandshakeBadResponse()
        {
            var provider = new HandshakeProvider();

            var transportMoq = new Mock<ITransport>();
            var context = new SmtpSessionContext()
            {
                ServerName = "localhost",
                Transport = transportMoq.Object

            };
            transportMoq.Setup(m => m.Read()).Returns("HELLO");

            var result = provider.RunAsync(context);

            Assert.AreEqual(500, result.StatusCode);
        }
    }
}
