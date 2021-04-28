﻿using Agent.antlr.ast.comparables;
using Agent.antlr.ast.comparables.subjects;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Agent.Tests.ast
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class SubjectTest
    {

        private const string Testname = "test";
        
        [Test]
        public void Test_GetNodeTypeSubject_CorrectOutput()
        {
            //Arrange
            var node = new Subject(Testname);
            //Act
            var result = node.GetNodeType();
            //Assert
            Assert.AreEqual("Subject", result);
        }
        
        [Test]
        public void Test_GetNodeTypeCurrent_CorrectOutput()
        {
            //Arrange
            var node = new Current(Testname);
            //Act
            var result = node.GetNodeType();
            //Assert
            Assert.AreEqual("Current", result);
        }
      
        [Test]
        public void Test_GetNodeTypeInventory_CorrectOutput()
        {
            //Arrange
            var node = new Inventory(Testname);
            //Act
            var result = node.GetNodeType();
            //Assert
            Assert.AreEqual("Inventory", result);
        }

        [Test]
        public void Test_GetNodeNPC_CorrectOutput()
        {
            //Arrange
            var node = new NPC(Testname);
            //Act
            var result = node.GetNodeType();
            //Assert
            Assert.AreEqual("NPC", result);
        }

        [Test]
        public void Test_GetNodeOpponent_CorrectOutput()
        {
            //Arrange
            var node = new Opponent(Testname);
            //Act
            var result = node.GetNodeType();
            //Assert
            Assert.AreEqual("Opponent", result);
        }

        [Test]
        public void Test_GetNodePlayer_CorrectOutput()
        {
            //Arrange
            var node = new Player(Testname);
            //Act
            var result = node.GetNodeType();
            //Assert
            Assert.AreEqual("Player", result);
        }

        [Test]
        public void Test_GetNodeTile_CorrectOutput()
        {
            //Arrange
            var node = new Tile(Testname);
            //Act
            var result = node.GetNodeType();
            //Assert
            Assert.AreEqual("Tile", result);
        }
        
    }
}