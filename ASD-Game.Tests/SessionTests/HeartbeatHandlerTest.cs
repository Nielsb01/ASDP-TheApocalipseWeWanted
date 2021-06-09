﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using Creature;
using Moq;
using ASD_Game.Session;
using NUnit.Framework;


namespace ASD_Game.Tests.SessionTests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class HeartbeatHandlerTest
    {
        //Declaration and initialisation of constant variables
        private HeartbeatHandler _sut;
        private StringWriter _stringWriter;
        private TextWriter _originalOutput;
        private Mock<IAgentHandler> _agentHandlerMock;

        [SetUp]
        public void Setup()
        {
            _agentHandlerMock = new Mock<IAgentHandler>();
            _sut = new HeartbeatHandler(_agentHandlerMock.Object);
            _stringWriter = new StringWriter();
            _originalOutput = Console.Out;
            Console.SetOut(_stringWriter);

        }

        [Test]
        public void Test_ReceiveHeartbeat_Success()
        {
            //arrange
            string expected = String.Empty;

            using (StringWriter sw = new StringWriter())
            {
                //Act ---------
                Console.SetOut(sw);
                _sut.ReceiveHeartbeat("test");
                //Assert ---------
                Assert.AreEqual(expected, sw.ToString());
            }

        }

        [Test]
        public void Test_ReceiveHeartbeat_Fail()
        {
            //arrange
            string expected = string.Format("Agents are enabled{0}", Environment.NewLine);

            using (StringWriter sw = new StringWriter())
            {
                //Act ---------
                Console.SetOut(sw);
                _sut.ReceiveHeartbeat("test");
                _sut.ReceiveHeartbeat("test2");
                Thread.Sleep(2000);
                _sut.ReceiveHeartbeat("test2");

                //Assert ---------
                Assert.AreEqual(expected, sw.ToString());
            }

        }
    }

}
