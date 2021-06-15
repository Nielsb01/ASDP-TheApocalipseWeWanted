﻿using ASD_Game.World.Models.Characters.StateMachine;
using Moq;
using NUnit.Framework;

namespace Creature.Tests
{
    [TestFixture]
    public class AgentTest
    {
        private World.Models.Characters.AgentAI _sut;
        private Mock<ICharacterStateMachine> _creatureStateMachineMock;

        private string Id;
        private string Name;
        private string Symbol;
        private int XPosition;
        private int YPosition;
        
        [SetUp]
        public void Setup()
        {
            _creatureStateMachineMock = new Mock<ICharacterStateMachine>();
            Id = "01";
            Name = "AgentTest";
            Symbol = "S";
            XPosition = 10;
            YPosition = 10;
            
            _sut = new World.Models.Characters.AgentAI(Name, XPosition, YPosition, Symbol, Id);
        }
        
        [Test]
        public void Test_CreateMonster_ObjectValuesAreSet()
        {
            // Assert ----------
            Assert.That(_sut.AgentStateMachine != null);
            Assert.That(_sut.Id == Id);
            Assert.That(_sut.Name == Name);
            Assert.That(_sut.Symbol == Symbol);
        }
        
    }
}