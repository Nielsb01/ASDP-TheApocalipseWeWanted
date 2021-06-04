﻿using Creature.Creature;
using Creature.Creature.NeuralNetworking.TrainingScenario;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using WorldGeneration.StateMachine.Data;

namespace Creature.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal class DataGatheringServiceTest
    {
        private DataGatheringServiceForTraining _sut;
        private SmartMonsterForTraining _smartMonster;
        private TrainerAI _player;

        [SetUp]
        public void Setup()
        {
            MonsterData _MonsterData =
                new MonsterData
                (
                14,
                14,
                0
                );
            _player = new TrainerAI(new Vector2(16, 16), "player");
            _smartMonster = new SmartMonsterForTraining("Zombie", 14, 14, "T", "monst");
            _sut = new DataGatheringServiceForTraining();
        }

        [Test]
        public void Test_ScanMap()
        {
            _sut.ScanMap(_smartMonster, _smartMonster.creatureData.VisionRange);

            Assert.NotNull(_sut.closestPlayer);
            Assert.NotNull(_sut.closestMonster);
        }

        [Test]
        public void Test_ScanMapPlayerAI_Adjacent()
        {
            _player.location = new Vector2(14, 15);

            Assert.NotNull(_sut.ScanMapPlayerAI(_player.location, _smartMonster));
        }

        [Test]
        public void Test_ScanMapPlayerAI_Not_Adjacent()
        {
            _player.location = new Vector2(20, 20);

            Assert.Null(_sut.ScanMapPlayerAI(_player.location, _smartMonster));
        }

        [Test]
        public void Test_CheckNewPosition()
        {
            _sut.CheckNewPosition(_smartMonster);

            int expected = -13;
            int actual = _smartMonster.score;

            Assert.AreEqual(expected, actual);
        }
    }
}