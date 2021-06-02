﻿using ActionHandling.DTO;
using DatabaseHandler.POCO;
using DatabaseHandler.Services;
using Items;
using Messages;
using Moq;
using Network;
using Network.DTO;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        
        [Test]
        public void Test_Inspect_CallsWorldService()
        {
            //arrange
            Player result = new(null, 0, 0, null, null);

            _mockedWorldService.Setup(mock => mock.GetCurrentPlayer()).Returns(result);

            //act
            _sut.InspectItem("helmet");

            //assert
            _mockedWorldService.Verify(mock => mock.GetCurrentPlayer(), Times.Once);
        }

        [Test]
        public void Test_Inspect_OutputsDescriptionHelmetSlot()
        {
            //arrange
            Player result = new(null, 0, 0, null, null);
            _mockedWorldService.Setup(mock => mock.GetCurrentPlayer()).Returns(result);
            string exp = $"Default headwear, plain but good looking{Environment.NewLine}Name: Bandana{Environment.NewLine}APP gain: 1{Environment.NewLine}Rarity: Common";
            
            //act
            _sut.InspectItem("helmet");
        
            //assert
            _mockedMessageService.Verify(mock => mock.AddMessage(exp), Times.Once);
        }
        
        [Test]
        public void Test_Inspect_OutputsDescriptionWeaponSlot()
        {
            //arrange
            Player result = new(null, 0, 0, null, null);
            _mockedWorldService.Setup(mock => mock.GetCurrentPlayer()).Returns(result);
            string exp = $"That ain't a knoife, this is a knoife{Environment.NewLine}Type: Melee{Environment.NewLine}Rarity: Common{Environment.NewLine}Damage: Low{Environment.NewLine}Attack speed: Slow";
            
            //act
            _sut.InspectItem("weapon");
        
            //assert
            _mockedMessageService.Verify(mock => mock.AddMessage(exp), Times.Once);
        }
        
        [Test]
        public void Test_Inspect_OutputsMessageOnEmptyArmorSlot()
        {
            //arrange
            Player result = new(null, 0, 0, null, null);
            _mockedWorldService.Setup(mock => mock.GetCurrentPlayer()).Returns(result);
            string exp = $"No item in this inventory slot";
            
            //act
            _sut.InspectItem("armor");
        
            //assert
            _mockedMessageService.Verify(mock => mock.AddMessage(exp), Times.Once);
        }
        
        [Test]
        public void Test_Inspect_OutputsDescriptionSlots()
        {
            //arrange
            Player result = new(null, 0, 0, null, null);
            result.Inventory.ConsumableItemList.Add(ItemFactory.GetBandage());
            _mockedWorldService.Setup(mock => mock.GetCurrentPlayer()).Returns(result);
            string exp = $"Let me patch you together{Environment.NewLine}Name: Bandage{Environment.NewLine}Rarity: Common{Environment.NewLine}Health gain: Low";
            
            //act
            _sut.InspectItem("slot 1");
        
            //assert
            _mockedMessageService.Verify(mock => mock.AddMessage(exp), Times.Once);
        }
        
        [Test]
        public void Test_Inspect_OutputsMessageOnEmptySlot1()
        {
            //arrange
            Player result = new(null, 0, 0, null, null);
            _mockedWorldService.Setup(mock => mock.GetCurrentPlayer()).Returns(result);
            string exp = $"No item in this inventory slot";
            
            //act
            _sut.InspectItem("slot 1");
        
            //assert
            _mockedMessageService.Verify(mock => mock.AddMessage(exp), Times.Once);
        }
        
        [Test]
        public void Test_Inspect_OutputsMessageOnEmptySlot2()
        {
            //arrange
            Player result = new(null, 0, 0, null, null);
            _mockedWorldService.Setup(mock => mock.GetCurrentPlayer()).Returns(result);
            string exp = $"No item in this inventory slot";
            
            //act
            _sut.InspectItem("slot 2");
        
            //assert
            _mockedMessageService.Verify(mock => mock.AddMessage(exp), Times.Once);
        }
        
        [Test]
        public void Test_Inspect_OutputsMessageOnEmptySlot3()
        {
            //arrange
            Player result = new(null, 0, 0, null, null);
            _mockedWorldService.Setup(mock => mock.GetCurrentPlayer()).Returns(result);
            string exp = $"No item in this inventory slot";
            
            //act
            _sut.InspectItem("slot 3");
        
            //assert
            _mockedMessageService.Verify(mock => mock.AddMessage(exp), Times.Once);
        }
    }
}
