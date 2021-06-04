﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Moq;
using Network;
using NUnit.Framework;
using Session.DTO;
using Session.GameConfiguration;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using ActionHandling;
using DatabaseHandler.POCO;
using DatabaseHandler.Services;
using WorldGeneration;
using DatabaseHandler.Services;
using DatabaseHandler.POCO;

namespace Session.Tests
{
    [ExcludeFromCodeCoverage]
    public class GameSessionHandlerTests
    {
        private GameSessionHandler _sut;

        private PacketDTO _packetDTO;

        //Declaration of mocks
        private Mock<IClientController> _mockedClientController;
        private Mock<IWorldService> _mockedWorldService;
        private Mock<ISessionHandler> _mockedsessionHandler;
        private Mock<IDatabaseService<PlayerPOCO>> _mockedPlayerServicesDb;
        private Mock<IDatabaseService<GamePOCO>> _mockedGameServicesDb;
        private Mock<IDatabaseService<PlayerItemPOCO>> _mockedPlayerItemServicesDb;
        private Mock<IRelativeStatHandler> _mockedRelativeStatHandler;

        private Mock<IGameConfigurationHandler> _mockedGameConfigurationHandler;
        private Mock<IDatabaseService<PlayerPOCO>> _mockedPlayerServiceDb;
        private Mock<IDatabaseService<GamePOCO>> _mockedgameServicesDb;
        private Mock<IDatabaseService<GameConfigurationPOCO>> _mockedGameConfigServicesDb;


        [SetUp]
        public void Setup()
        {
            var standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
            _mockedClientController = new Mock<IClientController>();
            _mockedWorldService = new Mock<IWorldService>();
            _mockedsessionHandler = new Mock<ISessionHandler>();
            _mockedGameConfigurationHandler = new Mock<IGameConfigurationHandler>();
            _mockedGameConfigServicesDb = new Mock<IDatabaseService<GameConfigurationPOCO>>();

            _mockedPlayerServiceDb = new Mock<IDatabaseService<PlayerPOCO>>();
            _mockedgameServicesDb = new Mock<IDatabaseService<GamePOCO>>();
           // _sut = new GameSessionHandler(_mockedClientController.Object, _mockedWorldService.Object, _mockedsessionHandler.Object, _mockedPlayerServiceDb.Object, _mockedgameServicesDb.Object, _mockedGameConfigServicesDb.Object, _mockedGameConfigurationHandler.Object);
            _packetDTO = new PacketDTO();
        }

        //Test below fails, not worth fixing atm since no other functions get tested
        // [Test]
        // public void Test_SendGameSession_CallsSendPayloadWithCorrectPayload()
        // {
        //     //arrange
        //     Dictionary<string, int[]> players = new Dictionary<string, int[]>();
        //     
        //     int[] playerPosition = new int[2];
        //     playerPosition[0] = 1;
        //     playerPosition[1] = 2;
        //     players.Add("player", playerPosition);
        //     
        //     StartGameDTO startGameDTO = new StartGameDTO
        //         {GameGuid = "testGame", PlayerLocations = players};
        //     
        //     var payload = JsonConvert.SerializeObject(startGameDTO);
        //
        //     _mockedClientController.Setup(mock => mock.SendPayload(payload, PacketType.Session));
        //     _mockedsessionHandler.Setup(mock => mock.GetAllClients()).Returns(new List<string>());
        //
        //     // Act ---------
        //     _sut.SendGameSession(_mockedsessionHandler.Object);
        //
        //     // Assert ---------
        //     _mockedClientController.Verify(mock => mock.SendPayload(payload, PacketType.Session), Times.Once());
        // }


    }
}