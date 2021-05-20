using Agent.Antlr.Ast;
using Agent.Antlr.Ast.Comparables;
using Agent.Antlr.Checker;
using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Agent.Tests.checker
{
    [ExcludeFromCodeCoverage]
    public class TestCheckerFunctions
    {
        private Checker _sut;

        [SetUp]
        public void Setup()
        {
            Mock<AST> ast = new Mock<AST>();
            this._sut = new Checker(ast.Object);
        }


        [Test]
        public void Test_CheckItemAndAllowedStat_1()
        {
            //Arrange
            Item item = new Item("Weapon");
            Stat stat = new Stat("Health");
            item.AddChild(stat);

            //Act
            bool result = this._sut.CheckItemAndAllowedStat(item);
            
            //Assert
            Assert.False(result);

        }
        
        [Test]
        public void Test_CheckStatCombination_2()
        {
            //Arrange
            List<Node> testNodes = new List<Node>();
            Item item = new Item("Weapon");
            Stat stat = new Stat("Health");
            item.AddChild(stat);
            
            When whenNode = new When();
            whenNode.SetComparableL(item);

            testNodes.Add(whenNode);

            //Act
            this._sut.CheckStatCombination(testNodes);
            
            //Assert
            Assert.AreNotEqual(String.Empty, item.GetError().ToString());
            Assert.AreEqual(new ASTError("There is an invalid combination of item and stat").ToString(), item.GetError().ToString());

        }
    }
}