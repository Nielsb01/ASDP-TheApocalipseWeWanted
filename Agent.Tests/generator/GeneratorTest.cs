﻿using Agent.antlr.ast;
using Moq;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Agent.Tests.generator
{
    [ExcludeFromCodeCoverage]
    public class GeneratorTest
    {

        private Generator sut;
        private Fixtures fix;

        [SetUp]
        public void Setup()
        {
            sut = new Generator();
            fix = new Fixtures();
        }

        [Test]
        [TestCase("test1.txt")]
        [TestCase("test2.txt")]
        [TestCase("test3.txt")]
        public void Test_Generator_Level1(String input)
        {
            //Arrange
            AST ast = fix.GetFixture(input);

            //Act
            var result = sut.Execute(ast);

            //Assert
            Assert.True(result.Length != 0);
            Assert.True(result.Contains("player"));
            Assert.True(result.Contains("nearby"));
            Assert.False(result.Contains("????"));
        }

        [Test]
        [TestCase("test3.txt")]
        public void Test_Generator_Exception(String input)
        {
            //Arrange
            AST ast = fix.GetFixture(input);

            Mock<Configuration> mockedNode = new();
            mockedNode.Setup(x => x.GetChildren()).Throws(new Exception());

            ast.SetRoot(mockedNode.Object);

            //Act
            var result = sut.Execute(ast);
            //Assert
            Assert.AreEqual(result, new Exception().Message);
        }
    }
}
