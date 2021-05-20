﻿using Agent.exceptions;
using Agent.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Tests
{
    class AgentConfigurationServiceTests
    {
        AgentConfigurationService sut;

        [SetUp]
        public void Setup()
        {
            sut = new AgentConfigurationService();
        }

        [Test]
        public void Test_StartConfiguration_SyntaxError()
        {
            //Arrange
            var input = String.Format(Path.GetFullPath(Path.Combine
                        (AppDomain.CurrentDomain.BaseDirectory, @"..\\..\\..\\"))) + "resource\\test_parse_exception.txt";

            Mock<ConsoleRetriever> mockedRetriever = new();
            mockedRetriever.SetupSequence(x => x.GetConsoleLine()).Returns(input).Returns("cancel");

            sut.ConsoleRetriever = mockedRetriever.Object;

            //Act
            sut.StartConfiguration();

            //Assert
            Assert.AreEqual("mismatched input '<EOF>' expecting '='", sut.testVar);
        }

        [Test]
        public void Test_StartConfiguration_Semantic()
        {
            //TODO not for this sprint bc of decision
        }
    }
}
