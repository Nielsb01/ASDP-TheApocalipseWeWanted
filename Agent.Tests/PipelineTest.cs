﻿using System;
using Moq;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Agent.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class PipelineTest
    {
        private static string SCRIPT = "combat{when player nearby player then attack}";
        private Pipeline sut;

        [SetUp]
        public void Setup()
        {
            sut = new Pipeline();
        }
        
        [Test]
        public void Test_ParseString_HappyPath()
        {
            //Arrange
            //Act
            sut.ParseString(SCRIPT);
            var result = sut.Ast;

            //Assert
            Assert.NotNull(result);
            Assert.IsEmpty(sut.Errors);
        }


        /*
        * ParseString()
        *
        * Parse incoming string
        * @author Abdul     
       */
        // [Test]
        // public void CheckAst()
        // {
            // //Arrange
            //
            // AST ast = new AST();
            // sut.Ast = ast;
            //
            // Mock<Checker> mockedChecker = new Mock<Checker>();
            // mockedChecker.Setup(mock => mock.Check(ast));
            //
            // sut.Checker = mockedChecker.Object;
            //
            // //Act
            // try
            // {
            //     sut.CheckAst();
            //     Assert.IsTrue(true);
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine(e);
            //     throw;
            // }
            //
            // //Assert
            // mockedChecker.Verify(mock => mock.Check(ast), Times.Once);
        // }
    }
}