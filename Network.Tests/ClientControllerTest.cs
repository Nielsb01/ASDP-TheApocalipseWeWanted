﻿using System;
using System.Diagnostics.CodeAnalysis;
using Moq;
using Network.DTO;
using NUnit.Framework;

namespace Network.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class ClientControllerTests
    {

        private ClientController _sut;
        private readonly string _SESSIONID = "1";
        
        private Mock<INetworkComponent> _mockedNetworkComponent;

        [SetUp]
        public void Setup()
        {
            _mockedNetworkComponent = new Mock<INetworkComponent>();
            _sut = new ClientController(_mockedNetworkComponent.Object);
        }

        [Test]
        public void Test_HandlePacket_SessionIsSame()
        {
            //Arrange
            var packetType = PacketType.Chat;
            var expectedHandlerResponse = new HandlerResponseDTO(SendAction.SendToClients, "test");
            var givenPacket = new PacketBuilder()
                .SetTarget("client")
                .SetPacketType(packetType)
                .SetPayload("random string")
                .SetSessionID(_SESSIONID)
                .Build();
            Mock<IPacketHandler> mockedPacketHandler = new Mock<IPacketHandler>();
            mockedPacketHandler.Setup(mock => mock.HandlePacket(givenPacket)).Returns(expectedHandlerResponse);
            _sut.SetSessionId(_SESSIONID);
            _sut.SubscribeToPacketType(mockedPacketHandler.Object, packetType);

            //Act
            var result = _sut.HandlePacket(givenPacket);

            //Assert
            mockedPacketHandler.Verify(mock => mock.HandlePacket(givenPacket), Times.Once);
            Assert.AreEqual(expectedHandlerResponse, result);
        }

        [Test]
        public void Test_HandlePacket_PacketTypeSession()
        {
            //Arrange
            var packetType = PacketType.Session;
            var expectedHandlerResponse = new HandlerResponseDTO(SendAction.SendToClients, "test");
            var givenPacket = new PacketBuilder()
                .SetTarget("client")
                .SetPacketType(packetType)
                .SetPayload("random string")
                .SetSessionID("not same")
                .Build();
            Mock<IPacketHandler> mockedPacketHandler = new Mock<IPacketHandler>();
            mockedPacketHandler.Setup(mock => mock.HandlePacket(givenPacket)).Returns(expectedHandlerResponse);
            _sut.SetSessionId(_SESSIONID);
            _sut.SubscribeToPacketType(mockedPacketHandler.Object, packetType);

            //Act
            var result = _sut.HandlePacket(givenPacket);

            //Assert
            mockedPacketHandler.Verify(mock => mock.HandlePacket(givenPacket), Times.Once);
            Assert.AreEqual(expectedHandlerResponse, result);
        }

        [Test]
        public void Test_HandlePacket_PacketTypeSessionAndSessionIsSame()
        {
            //Arrange
            var packetType = PacketType.Session;
            var expectedHandlerResponse = new HandlerResponseDTO(SendAction.SendToClients, "test");
            var givenPacket = new PacketBuilder()
                .SetTarget("client")
                .SetPacketType(packetType)
                .SetPayload("random string")
                .SetSessionID(_SESSIONID)
                .Build();
            Mock<IPacketHandler> mockedPacketHandler = new Mock<IPacketHandler>();
            mockedPacketHandler.Setup(mock => mock.HandlePacket(givenPacket)).Returns(expectedHandlerResponse);
            _sut.SetSessionId(_SESSIONID);
            _sut.SubscribeToPacketType(mockedPacketHandler.Object, packetType);

            //Act
            var result = _sut.HandlePacket(givenPacket);

            //Assert
            mockedPacketHandler.Verify(mock => mock.HandlePacket(givenPacket), Times.Once);
            Assert.AreEqual(expectedHandlerResponse, result);
        }

        [Test]
        public void Test_HandlePacket_PacketTypeNotSessionAndSessionIsNotSame()
        {
            //Arrange
            var packetType = PacketType.Chat;
            var expectedHandlerResponse = new HandlerResponseDTO(SendAction.ReturnToSender, null);
            var givenPacket = new PacketBuilder()
                .SetTarget("client")
                .SetPacketType(packetType)
                .SetPayload("random string")
                .SetSessionID("not same")
                .Build();
            Mock<IPacketHandler> mockedPacketHandler = new Mock<IPacketHandler>();
            mockedPacketHandler.Setup(mock => mock.HandlePacket(givenPacket)).Returns(expectedHandlerResponse);
            _sut.SetSessionId(_SESSIONID);
            _sut.SubscribeToPacketType(mockedPacketHandler.Object, packetType);

            //Act
            var result = _sut.HandlePacket(givenPacket);

            //Assert
            mockedPacketHandler.Verify(mock => mock.HandlePacket(givenPacket), Times.Never);
            Assert.AreEqual(expectedHandlerResponse, result);
        }

        [Test]
        public void Test_SendPayload_PayloadIsNull()
        {
            //Arrange
            string expectedExceptionMessage = "Payload is empty.";
            string payload = null;
            var packetType = PacketType.Chat;
            
            //Act
            var ex = Assert.Throws<Exception>(() => _sut.SendPayload(payload, packetType));

            //Assert
            Assert.AreEqual(expectedExceptionMessage, ex.Message);
        }

        [Test]
        public void Test_SendPayload_PayloadIsEmpty()
        {
            //Arrange
            string expectedExceptionMessage = "Payload is empty.";
            string payload = "";
            var packetType = PacketType.Chat;

            //Act
            var ex = Assert.Throws<Exception>(() => _sut.SendPayload(payload, packetType));

            //Assert
            Assert.AreEqual(expectedExceptionMessage, ex.Message);
        }

        [Test]
        public void Test_SendPayload_WithoutHostController()
        {
            //Arrange
            string payload = "random string";
            var packetType = PacketType.Chat;
            _mockedNetworkComponent.Setup(mock => mock.SendPacket(It.IsAny<PacketDTO>()));

            //Act
            _sut.SendPayload(payload, packetType);

            //Assert
            _mockedNetworkComponent.Verify(mock => mock.SendPacket(It.IsAny<PacketDTO>()), Times.Once);
        }

        [Test]
        public void Test_SendPayload_WithHostController()
        {
            //Arrange
            var mockedHostController = new Mock<IHostController>();
            string payload = "random string";
            var packetType = PacketType.Chat;
            mockedHostController.Setup(mock => mock.ReceivePacket(It.IsAny<PacketDTO>()));
            _sut.SetHostController(mockedHostController.Object);

            //Act
            _sut.SendPayload(payload, packetType);

            //Assert
            mockedHostController.Verify(mock => mock.ReceivePacket(It.IsAny<PacketDTO>()), Times.Once);
        }

        [Test]
        public void Test_SetSessionId_WhenHostControllerIsSet()
        {
            //Arrange
            Mock<IHostController> mockedHostController = new Mock<IHostController>();
            _sut.SetHostController(mockedHostController.Object);
            
            //Act
            _sut.SetSessionId(_SESSIONID);
            
            //Assert
            mockedHostController.Verify(mock => mock.SetSessionId(_SESSIONID), Times.Once);
        }
    }
}
