﻿using NUnit.Framework;
using Moq;
using System.Numerics;
using Creature.Creature.StateMachine.Data;
using System.Diagnostics.CodeAnalysis;
using Creature.Creature;
using Creature.Creature.StateMachine;

namespace Creature.Tests
{
    [ExcludeFromCodeCoverage]
    class MonsterTest
    {
        private Monster _sut;
        private Mock<ICreatureStateMachine> _creatureStateMachineMock;

        [SetUp]
        public void Setup()
        {
            _creatureStateMachineMock = new Mock<ICreatureStateMachine>();
            _sut = new Monster(_creatureStateMachineMock.Object);
        }

        [Test]
        public void Test_MonsterStateMachine_StartsStateMachine()
        {
            // Assert ----------
            _creatureStateMachineMock.Verify(creatureStateMachine => creatureStateMachine.StartStateMachine());
        }

        [Test]
        public void Test_ApplyDamage_DealsDamage()
        {
            // Arrange ---------
            MonsterData monsterData = new MonsterData(new Vector2(), 50, 10, 10, null, false);
            _creatureStateMachineMock.Setup(creatureStateMachine => creatureStateMachine.CreatureData)
                .Returns(monsterData);

            // Act -------------
            _sut.ApplyDamage(30);

            // Assert ----------
            Assert.AreEqual(_sut.CreatureStateMachine.CreatureData.Health, 20);
        }

        [Test]
        public void Test_HealAmount_HealsMonster()
        {
            // Arrange ---------
            MonsterData monsterData = new MonsterData(new Vector2(), 30, 10, 10, null, false);
            _creatureStateMachineMock.Setup(creatureStateMachine => creatureStateMachine.CreatureData)
                .Returns(monsterData);

            // Act -------------
            _sut.HealAmount(10);

            // Assert ----------
            Assert.AreEqual(_sut.CreatureStateMachine.CreatureData.Health, 40);
        }
    }
}