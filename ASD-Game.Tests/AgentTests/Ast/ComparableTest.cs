﻿using Agent.Antlr.Ast;
using Agent.Antlr.Ast.Comparables;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Agent.Tests.Ast
{
    [ExcludeFromCodeCoverage]

    [TestFixture]
    public class ComparableTest
    {
        private Comparable _comparable;



        [Test]
        public void Test_GetNodeTypeComparable_CorrectOutput()
        {
            //Arrange
            _comparable = new Comparable();
            //Act
            var result = _comparable.GetNodeType();
            //Assert
            Assert.AreEqual("Comparable", result);
        }



        [Test]
        public void Test_GetNodeTypeItem_CorrectOutput()
        {
            //Arrange
            _comparable = new Item("");
            //Act
            var result = _comparable.GetNodeType();
            //Assert
            Assert.AreEqual("Item", result);
        }


        [Test]
        public void Test_GetNodeTypeInt_CorrectOutput()
        {
            //Arrange
            _comparable = new Int(1);
            //Act
            var result = _comparable.GetNodeType();
            //Assert
            Assert.AreEqual("Int", result);
        }


        [Test]
        public void Test_GetNodeTypeStat_CorrectOutput()
        {
            //Arrange
            _comparable = new Stat("");
            //Act
            var result = _comparable.GetNodeType();
            //Assert
            Assert.AreEqual("Stat", result);
        }


        [Test]
        public void Test_GetNodeTypeSubject_CorrectOutput()
        {
            //Arrange
            _comparable = new Subject("");
            //Act
            var result = _comparable.GetNodeType();
            //Assert
            Assert.AreEqual("Subject", result);
        }
    }
}