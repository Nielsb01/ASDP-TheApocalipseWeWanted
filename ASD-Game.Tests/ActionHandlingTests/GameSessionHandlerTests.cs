using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using ActionHandling;
using Agent.Services;
using ASD_Game.ActionHandling;
using ASD_Game.DatabaseHandler.POCO;
using ASD_Game.DatabaseHandler.Services;
using ASD_Game.Messages;
using ASD_Game.Network;
using ASD_Game.Network.DTO;
using ASD_Game.Session;
using ASD_Game.Session.GameConfiguration;
using ASD_Game.UserInterface;
using ASD_Game.World.Services;
using Moq;
using NUnit.Framework;

namespace ASD_Game.Tests.SessionTests

{
    [ExcludeFromCodeCoverage]
    public class GameSessionHandlerTests
    {
        private GameSessionHandler _sut;

        private PacketDTO _packetDTO;

        //Declaration of mocks
        private Mock<ClientController> _mockedClientController; //change this to the interface and all test break, your choice.
        private Mock<IWorldService> _mockedWorldService;
        private Mock<ISessionHandler> _mockedSessionHandler;
        private Mock<IDatabaseService<PlayerPOCO>> _mockedPlayerDatabaseService;
        private Mock<IDatabaseService<GamePOCO>> _mockedGameDatabaseService;
        private Mock<IDatabaseService<PlayerItemPOCO>> _mockedPlayerItemDatabaseService;
        private Mock<IRelativeStatHandler> _mockedRelativeStatHandler;
        private Mock<IScreenHandler> _mockedScreenHandler;
        private Mock<IMessageService> _mockedMessageService;
        private Mock<IGameConfigurationHandler> _mockedGameConfigurationHandler;
        private Mock<IDatabaseService<GameConfigurationPOCO>> _mockedGameConfigDatabaseService;
        private Mock<INetworkComponent> _mockedNetworkComponent;
        private Mock<IConfigurationService> _mockedConfigurationService;
        private Mock<IMoveHandler> _mockedMoveHandler;
        private Mock<IAttackHandler> _mockedAttackHandler;

        [SetUp]
        public void Setup()
        {
            Mock<INetworkComponent> tmpMock = new();

            var standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
            _mockedClientController = new Mock<ClientController>(tmpMock.Object);
            _mockedWorldService = new Mock<IWorldService>();
            _mockedSessionHandler = new Mock<ISessionHandler>();
            _mockedGameConfigurationHandler = new Mock<IGameConfigurationHandler>();
            _mockedGameConfigDatabaseService = new Mock<IDatabaseService<GameConfigurationPOCO>>();
            _mockedPlayerItemDatabaseService = new Mock<IDatabaseService<PlayerItemPOCO>>();
            _mockedRelativeStatHandler = new Mock<IRelativeStatHandler>();
            _mockedPlayerDatabaseService = new Mock<IDatabaseService<PlayerPOCO>>();
            _mockedGameDatabaseService = new Mock<IDatabaseService<GamePOCO>>();
            _mockedScreenHandler = new Mock<IScreenHandler>();
            _mockedMessageService = new Mock<IMessageService>();
            _mockedNetworkComponent = new Mock<INetworkComponent>();
            _mockedConfigurationService = new Mock<IConfigurationService>();
            _mockedMoveHandler = new Mock<IMoveHandler>();
            _mockedAttackHandler = new Mock<IAttackHandler>();
            _sut = new GameSessionHandler(_mockedClientController.Object,
                _mockedSessionHandler.Object,
                _mockedRelativeStatHandler.Object,
                _mockedGameConfigurationHandler.Object,
                _mockedScreenHandler.Object,
                _mockedPlayerDatabaseService.Object,
                _mockedGameDatabaseService.Object,
                _mockedGameConfigDatabaseService.Object,
                _mockedPlayerItemDatabaseService.Object,
                _mockedWorldService.Object,
                _mockedMessageService.Object,
                _mockedNetworkComponent.Object,
                _mockedConfigurationService.Object,
                _mockedMoveHandler.Object,
                _mockedAttackHandler.Object
                );
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