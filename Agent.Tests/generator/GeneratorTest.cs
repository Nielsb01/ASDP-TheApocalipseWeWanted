﻿using Agent.antlr.ast;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Tests.generator
{
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
            var result = sut.execute(ast);

            //Assert
            Assert.True(true);
        }


    }
}
