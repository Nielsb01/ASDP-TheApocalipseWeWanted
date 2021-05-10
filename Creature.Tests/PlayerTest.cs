﻿using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Creature.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class PlayerTest
    {
        private ICreature _sut;

        [SetUp]
        public void Setup()
        {
            Vector2 position = new Vector2(10, 10);
            int health = 20;

            _sut = new Player(health, position);
        }

        [Test]
        public void Test_ApplyDamage_KillsPlayer()
        {
            // Arrange ---------

            // Act -------------
            _sut.ApplyDamage(30);

            // Assert ----------
            Assert.That(_sut.IsAlive == false);
        }

        [Test]
        public void Test_HealAmount_DoesNotRevivePlayer()
        {
            // Arrange ---------

            // Act -------------
            _sut.ApplyDamage(30);
            _sut.HealAmount(50);

            // Assert ----------
            Assert.That(_sut.IsAlive == false);
        }
    }
}
