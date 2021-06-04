using System.Diagnostics.CodeAnalysis;
using ActionHandling;
using ActionHandling.DTO;
using DatabaseHandler.POCO;
using DatabaseHandler.Services;
using Moq;
using Network;
using Newtonsoft.Json;
using NUnit.Framework;
using WorldGeneration;

namespace Session.Tests
{
    [ExcludeFromCodeCoverage]
    public class RelativeStatHandlerTest
    {
        private Mock<IClientController> _mockedClientController;
        private Mock<IWorldService> _mockedWorldService;
        private Mock<IDatabaseService<PlayerPOCO>> _mockedPlayerServicesDb;
        private RelativeStatHandler _sut;

        [SetUp]
        public void Setup()
        {
            _mockedClientController = new Mock<IClientController>();
            _mockedWorldService = new Mock<IWorldService>();

            _sut = new RelativeStatHandler(_mockedClientController.Object, _mockedWorldService.Object, _mockedPlayerServicesDb.Object);
        }

        [Test]
        public void Test_SendStat_SendPayloadCorrectly()
        {
            //Arrange
            var dto = new RelativeStatDTO();
            dto.Id = "testId";
            dto.Stamina = 5;

            var payload = JsonConvert.SerializeObject(dto);

            _mockedClientController.Setup(mock => mock.GetOriginId()).Returns("testId");
            _mockedClientController.Setup(mock => mock.SendPayload(payload, PacketType.RelativeStat));

            //Act
            _sut.SendStat(dto);
            
            //Assert
            _mockedClientController.Verify(mock => mock.GetOriginId(), Times.Once);
            _mockedClientController.Verify(mock => mock.SendPayload(payload, PacketType.RelativeStat), Times.Once);
        }
    }
}