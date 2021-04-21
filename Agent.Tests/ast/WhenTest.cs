﻿using Agent.antlr.ast;
using Agent.antlr.ast.comparables;
using Agent.antlr.ast.comparables.subjects;
using NUnit.Framework;
using System;
using Comparable = Agent.antlr.ast.Comparable;

namespace Agent.Tests.ast
{
    [TestFixture]
    public class WhenTest
    {

        private When _sut;
        private const string Type = "When";
        
        [SetUp]
        public void Setup()
        {
            this._sut = new When();
        }
        
        /*
         * GetNodeType()
         *
         * Test of de juiste type terug gegeven wordt
         */
        [Test]
        public void GetNodeTypeTest()
        {
            //Arrange
            
            //Act
            var result = this._sut.GetNodeType();
            //Assert
            Assert.AreEqual(result, Type);
        }
        
        /*
        * AddChild()
        *
        * Test of de eerste comparable als de linker comparable goed geplaatst wordt.
        */
        [Test]
        [TestCase(typeof(Current))]
        [TestCase(typeof(Inventory))]
        [TestCase(typeof(NPC))]
        [TestCase(typeof(Opponent))]
        [TestCase(typeof(Player))]
        [TestCase(typeof(Tile))]
        [TestCase(typeof(Int))]
        [TestCase(typeof(Item))]
        [TestCase(typeof(Stat))]
        [TestCase(typeof(Subject))]
        [TestCase(typeof(Comparable))]
        public void AddChildTest1( Type testCase)
        {
            //Arrange
            var comparable = ComparableTestCase(testCase);
            
            //Act
            this._sut.AddChild( comparable);
            //Assert
            
            Assert.AreEqual( comparable, this._sut.GetComparableL());
            Assert.AreEqual(null,this._sut.GetComparableR());
            Assert.AreEqual(null,this._sut.GetComparison());
            Assert.AreEqual(null,this._sut.GetThen());
            Assert.AreEqual(1, this._sut.GetChildren().Count);
        }
        
        /*
        * AddChild()
        *
        * Test of als er twee comparables toegevoegd worden de tweede de rechter comaprable is.
        */
        [Test]
        [TestCase(typeof(Current))]
        [TestCase(typeof(Inventory))]
        [TestCase(typeof(NPC))]
        [TestCase(typeof(Opponent))]
        [TestCase(typeof(Player))]
        [TestCase(typeof(Tile))]
        [TestCase(typeof(Int))]
        [TestCase(typeof(Item))]
        [TestCase(typeof(Stat))]
        [TestCase(typeof(Subject))]
        [TestCase(typeof(Comparable))]
        public void AddChildTest2( Type testCase)
        {
            //Arrange
            var comparableL = new Comparable();
            var comparableR = ComparableTestCase(testCase);
            this._sut.AddChild(comparableL);
            //Act
            this._sut.AddChild(comparableR);
            //Assert
            
            Assert.AreEqual(comparableL, this._sut.GetComparableL());
            Assert.AreEqual(comparableR,this._sut.GetComparableR());
            Assert.AreEqual(null,this._sut.GetComparison());
            Assert.AreEqual(null,this._sut.GetThen());
            Assert.AreEqual(2, this._sut.GetChildren().Count);
        }
        /*
        * AddChild()
        *
        * Test of als er al twee comparables zijn toegevoegd de volgende aan de body toegevoegd wordt.
        */
        [Test]
        [TestCase(typeof(Current))]
        [TestCase(typeof(Inventory))]
        [TestCase(typeof(NPC))]
        [TestCase(typeof(Opponent))]
        [TestCase(typeof(Player))]
        [TestCase(typeof(Tile))]
        [TestCase(typeof(Int))]
        [TestCase(typeof(Item))]
        [TestCase(typeof(Stat))]
        [TestCase(typeof(Subject))]
        [TestCase(typeof(Comparable))]
        public void AddChildTest3( Type testCase)
        {
            //Arrange
            var comparableL = new Comparable();
            var comparableR = new Comparable();
            var comparable = ComparableTestCase(testCase);
            this._sut.AddChild(comparableL);
            this._sut.AddChild(comparableR);
            //Act
            this._sut.AddChild(comparable);
            //Assert
            
            Assert.AreEqual(comparableL, this._sut.GetComparableL());
            Assert.AreEqual(comparableR,this._sut.GetComparableR());
            Assert.AreEqual(null,this._sut.GetComparison());
            Assert.AreEqual(null,this._sut.GetThen());
            Assert.AreEqual(3, this._sut.GetChildren().Count);
            Assert.AreEqual(comparable, this._sut.GetChildren()[2]);
        }
        
        /*
        * AddChild()
        *
        * Test of de then actionreference goed wordt toegevoegd.
        */
        [Test]
        public void AddChildTest4()
        {
            //Arrange
            var action = new ActionReference("test");
            //Act
            this._sut.AddChild(action);
            //Assert
            Assert.AreEqual(null, this._sut.GetComparableL());
            Assert.AreEqual(null,this._sut.GetComparableR());
            Assert.AreEqual(null,this._sut.GetComparison());
            Assert.AreEqual(action,this._sut.GetThen());
            Assert.AreEqual(1, this._sut.GetChildren().Count);
        }
        
        /*
        * AddChild()
        *
        * Test of de comparison goed wordt toegevoegd.
        */
        [Test]
        public void AddChildTest5()
        {
            //Arrange
            var comparison = new Comparison("test");
            //Act
            this._sut.AddChild(comparison);
            //Assert
            Assert.AreEqual(null, this._sut.GetComparableL());
            Assert.AreEqual(null,this._sut.GetComparableR());
            Assert.AreEqual(comparison,this._sut.GetComparison());
            Assert.AreEqual(null,this._sut.GetThen());
            
            Assert.AreEqual(1, this._sut.GetChildren().Count);
        }
        
                
        /*
        * AddChild()
        *
        * Test of de comparison goed wordt toegevoegd.
        */
        [Test]
        public void AddChildTest6()
        {
            //Arrange
            var extra = new Node();
            //Act
            this._sut.AddChild(extra);
            //Assert
            Assert.AreEqual(null, this._sut.GetComparableL());
            Assert.AreEqual(null,this._sut.GetComparableR());
            Assert.AreEqual(null,this._sut.GetComparison());
            Assert.AreEqual(null,this._sut.GetThen());
            Assert.AreEqual(1, this._sut.GetChildren().Count);
            Assert.AreEqual(extra, this._sut.GetChildren()[0]);
        }
        
        private static Comparable ComparableTestCase(Type testCase)
        {
            Comparable comparable;
            if (testCase == typeof(Int)){
                comparable = (Comparable) Activator.CreateInstance(testCase,1);
            }
            else if (testCase == typeof(Comparable)){
                comparable = (Comparable) Activator.CreateInstance(testCase);
            }
            else {
                comparable = (Comparable) Activator.CreateInstance(testCase, "test");
            }

            return comparable;
        }
        

        
        
        
    }
}