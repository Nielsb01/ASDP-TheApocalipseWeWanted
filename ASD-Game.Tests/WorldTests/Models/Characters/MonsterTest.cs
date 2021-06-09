﻿using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using World.Models.Characters.StateMachine.Data;
using WorldGeneration;
using WorldGeneration.Models;

namespace Character.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal class MonsterTest
    {
        private Monster _sut;

        private int _xPosition, _yPosition;

        [SetUp]
        public void Setup()
        {
            _xPosition = 15;
            _yPosition = 20;
        }

        [Test]
        public void Test_Instantiate_MonsterDataGetsCreated()
        {
            //Arrange
            MonsterData expectedCharacterData = new MonsterData(_xPosition, _yPosition, 0);

            //Act
            _sut = new Monster("Zombie", 15, 20, CharacterSymbol.TERMINATOR, "monster1");

            //Assert
            Assert.AreEqual(expectedCharacterData.Position.X, _sut.MonsterData.Position.X);
            Assert.AreEqual(expectedCharacterData.Position.Y, _sut.MonsterData.Position.Y);
            Assert.AreEqual(expectedCharacterData.Health, _sut.MonsterData.Health);
            Assert.AreEqual(expectedCharacterData.Damage, _sut.MonsterData.Damage);
        }
    }
}