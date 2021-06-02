﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ActionHandling.DTO;
using DatabaseHandler.POCO;
using DatabaseHandler.Services;
using Items;
using Messages;
using Moq;
using Network;
using Network.DTO;
using Newtonsoft.Json;
using NUnit.Framework;
using WorldGeneration;

namespace ActionHandling.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class InventoryHandlerTest
    {
        private InventoryHandler _sut;
        private Mock<IClientController> _mockedClientController;
        private Mock<IWorldService> _mockedWorldService;
        private Mock<IMessageService> _mockedMessageService;
        private Mock<IServicesDb<PlayerPOCO>> _mockedPlayerServicesDb;
        private Mock<IServicesDb<PlayerItemPOCO>> _mockedPlayerItemServicesDb;

        [SetUp]
        public void Setup()
        {
            _mockedClientController = new();
            _mockedWorldService = new();
            _mockedPlayerServicesDb = new();
            _mockedPlayerItemServicesDb = new();
            _mockedMessageService = new();

            _sut = new InventoryHandler(_mockedClientController.Object, _mockedWorldService.Object, _mockedPlayerServicesDb.Object, _mockedPlayerItemServicesDb.Object, _mockedMessageService.Object);
        }

        [Test]
        public void Test_Search_CallsWorldService()
        {
            //arrange
            string exampleResult = "result";

            _mockedWorldService.Setup(mock => mock.SearchCurrentTile()).Returns(exampleResult);

            //act
            _sut.Search();

            //assert
            _mockedWorldService.Verify(mock => mock.SearchCurrentTile(), Times.Once);
        }

        [Test]
        public void Test_Use_SendsInventoryDTO()
        {
            //arrange
            int index = 1;
            string originId = "origin1";



            _mockedClientController.Setup(mock => mock.GetOriginId()).Returns(originId);

            InventoryDTO inventoryDTO = new(originId, InventoryType.Use, index);
            var payload = JsonConvert.SerializeObject(inventoryDTO);

            //act
            _sut.UseItem(index);

            //assert
            _mockedClientController.Verify(mock => mock.SendPayload(payload, PacketType.Inventory), Times.Once);
        }

        [Test]
        public void Test_Pickup_SendsInventoryDTO()
        {
            // Arrange
            const int INDEX = 1;
            const int COMPENSATED_INDEX = 0;
            string originId = "origin1";
            _mockedClientController.Setup(mock => mock.GetOriginId()).Returns(originId);
            
            InventoryDTO inventoryDTO = new(originId, InventoryType.Pickup, COMPENSATED_INDEX);
            string payload = JsonConvert.SerializeObject(inventoryDTO);
            
            // Act
            _sut.PickupItem(INDEX);
            
            // Assert
            _mockedClientController.Verify(mock => mock.SendPayload(payload, PacketType.Inventory), Times.Once);
        }

        [Test]
        [TestCaseSource(typeof(HandlesPickupPacketCases))]
        public void Test_HandlePacket_HandlesPickupPacket(InventoryDTO inventoryDTO, HandlerResponseDTO expectedHandlerResponseDTO, bool filledInventory, bool asHost)
        {
            // Arrange
            string payload = JsonConvert.SerializeObject(inventoryDTO);
            PacketDTO packetDTO = new();
            packetDTO.Payload = payload;
            PacketHeaderDTO packetHeaderDTO = new PacketHeaderDTO();
            packetHeaderDTO.Target = "host";
            packetDTO.Header = packetHeaderDTO;

            Player player = new Player("henk", 0, 0, "#", inventoryDTO.UserId);
            IList<Item> items = new List<Item>();
            items.Add(ItemFactory.GetBandage());
            if (filledInventory)
            {
                player.Inventory.AddConsumableItem(ItemFactory.GetBandage());
                player.Inventory.AddConsumableItem(ItemFactory.GetBandage());
                player.Inventory.AddConsumableItem(ItemFactory.GetBandage());
            }

            _mockedWorldService.Setup(mock => mock.GetPlayer(inventoryDTO.UserId)).Returns(player);
            _mockedWorldService.Setup(mock => mock.GetItemsOnCurrentTile(player)).Returns(items);
            if (asHost)
            {
                _mockedClientController.Setup(mock => mock.IsHost()).Returns(true);
            }
            
            // Act
            HandlerResponseDTO handlerResponseDTO = _sut.HandlePacket(packetDTO);

            // Assert
            Assert.AreEqual(expectedHandlerResponseDTO, handlerResponseDTO);
            if (asHost)
            {
                _mockedPlayerItemServicesDb.Verify(mock => mock.CreateAsync(It.IsAny<PlayerItemPOCO>()), Times.Once());
            }
        }
        
        class HandlesPickupPacketCases : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new object[]
                {
                    new InventoryDTO("userid", InventoryType.Pickup, 0),
                    new HandlerResponseDTO(SendAction.SendToClients, null),
                    false,
                    true
                };
                yield return new object[]
                {
                    new InventoryDTO("userid", InventoryType.Pickup, 100),
                    new HandlerResponseDTO(SendAction.ReturnToSender, "Number is not in search list!"),
                    false,
                    false
                };
                yield return new object[]
                {
                    new InventoryDTO("userid", InventoryType.Pickup, 0),
                    new HandlerResponseDTO(SendAction.ReturnToSender, "Could not pickup item"),
                    true,
                    false
                };
            }
        }

        [Test]
        public void Test_HandlePacket_HandlesUsePacketSuccesOnHost()
        {
            //arrange
            bool isHost = true;
            string originId = "origin1";
            int index = 1;
            
            InventoryDTO inventoryDTO = new(originId, InventoryType.Use, index);

            var payload = JsonConvert.SerializeObject(inventoryDTO);
            PacketDTO packetDTO = new();
            packetDTO.Payload = payload;
            PacketHeaderDTO packetHeaderDTO = new PacketHeaderDTO();
            packetHeaderDTO.Target = "host";
            packetDTO.Header = packetHeaderDTO;

            Player player = new("arie", 0, 0, "#", originId);
            player.Health = 50;
            var item = ItemFactory.GetBandage();
            player.Inventory.AddConsumableItem(item);

            PlayerPOCO playerPOCO = new() { PlayerGuid = originId, Health = 50 };
            List<PlayerPOCO> playerPOCOList = new();
            playerPOCOList.Add(playerPOCO);
            IEnumerable<PlayerPOCO> en = playerPOCOList;
            var task = Task.FromResult(en);
            

            _mockedWorldService.Setup(mock => mock.GetPlayer(originId)).Returns(player);
            _mockedClientController.Setup(mock => mock.IsHost()).Returns(isHost);
            _mockedPlayerServicesDb.Setup(mock => mock.GetAllAsync()).Returns(task);

            var expectedResult = new HandlerResponseDTO(SendAction.SendToClients, null);

            //act
            var result = _sut.HandlePacket(packetDTO);



            //assert
            playerPOCO.Health += 25;
            Assert.IsTrue(player.Inventory.ConsumableItemList.Count == 0);
            Assert.IsTrue(player.Health == 75);
            Assert.AreEqual(expectedResult, result);
            _mockedPlayerServicesDb.Verify(mock => mock.UpdateAsync(playerPOCO), Times.Once);
            _mockedPlayerItemServicesDb.Verify(mock => mock.DeleteAsync(It.IsAny<PlayerItemPOCO>()), Times.Once);
        }

        [Test]
        public void Test_HandlePacket_HandlesUsePacketFailsOnHost()
        {
            //arrange
            bool isHost = true;
            string originId = "origin1";
            int index = 1;

            InventoryDTO inventoryDTO = new(originId, InventoryType.Use, index);

            var payload = JsonConvert.SerializeObject(inventoryDTO);
            PacketDTO packetDTO = new();
            packetDTO.Payload = payload;
            PacketHeaderDTO packetHeaderDTO = new PacketHeaderDTO();
            packetHeaderDTO.Target = "host";
            packetDTO.Header = packetHeaderDTO;

            Player player = new("arie", 0, 0, "#", originId);
            player.Health = 50;
            var item = ItemFactory.GetBandage();

            PlayerPOCO playerPOCO = new() { PlayerGuid = originId, Health = 50 };
            List<PlayerPOCO> playerPOCOList = new();
            playerPOCOList.Add(playerPOCO);
            IEnumerable<PlayerPOCO> en = playerPOCOList;
            var task = Task.FromResult(en);


            _mockedWorldService.Setup(mock => mock.GetPlayer(originId)).Returns(player);
            _mockedClientController.Setup(mock => mock.IsHost()).Returns(isHost);
            _mockedPlayerServicesDb.Setup(mock => mock.GetAllAsync()).Returns(task);
            _mockedClientController.Setup(mock => mock.GetOriginId()).Returns(originId);

            var expectedResult = new HandlerResponseDTO(SendAction.ReturnToSender, "Could not find item");

            //act
            var result = _sut.HandlePacket(packetDTO);

            //assert
            Assert.IsTrue(player.Inventory.ConsumableItemList.Count == 0);
            Assert.IsTrue(player.Health == 50);
            Assert.AreEqual(expectedResult, result);
            _mockedPlayerServicesDb.Verify(mock => mock.UpdateAsync(playerPOCO), Times.Never);
            _mockedPlayerItemServicesDb.Verify(mock => mock.DeleteAsync(It.IsAny<PlayerItemPOCO>()), Times.Never);
        }
    }
}
