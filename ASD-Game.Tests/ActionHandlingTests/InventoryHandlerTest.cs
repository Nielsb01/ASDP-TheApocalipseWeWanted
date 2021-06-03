﻿using System;
using System.Collections;
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
        private Mock<IServicesDb<WorldItemPOCO>> _mockedWorldItemServicesDb;

        [SetUp]
        public void Setup()
        {
            _mockedClientController = new();
            _mockedWorldService = new();
            _mockedPlayerServicesDb = new();
            _mockedPlayerItemServicesDb = new();
            _mockedMessageService = new();
            _mockedWorldItemServicesDb = new();

            _sut = new InventoryHandler(_mockedClientController.Object, _mockedWorldService.Object, _mockedPlayerServicesDb.Object, _mockedPlayerItemServicesDb.Object, _mockedWorldItemServicesDb.Object, _mockedMessageService.Object);
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
        public void Test_DropItem_SendsInventoryDTO()
        {
            // Arrange
            const string ITEM = "helmet";
            string originId = "origin1";
            _mockedClientController.Setup(mock => mock.GetOriginId()).Returns(originId);

            InventoryDTO inventoryDTO = new(originId, InventoryType.Drop, 0);
            string payload = JsonConvert.SerializeObject(inventoryDTO);

            // Act
            _sut.DropItem(ITEM);

            // Assert
            _mockedClientController.Verify(mock => mock.SendPayload(payload, PacketType.Inventory), Times.Once);
        }

        [Test]
        public void Test_DropItem_SendsErrorMessage()
        {
            // Arrange
            const string ITEM = "helmet99";
            string originId = "origin1";
            _mockedClientController.Setup(mock => mock.GetOriginId()).Returns(originId);

            InventoryDTO inventoryDTO = new(originId, InventoryType.Drop, 99);
            string payload = JsonConvert.SerializeObject(inventoryDTO);

            // Act
            _sut.DropItem(ITEM);

            // Assert
            _mockedMessageService.Verify(mock => mock.AddMessage("Unknown item slot"), Times.Once);
        }

        [Test]
        [TestCaseSource(typeof(HandlesDropPacketCases))]
        public void Test_HandlePacket_HandlesDropPacket(PacketDTO packetDTO, InventoryDTO inventoryDTO, string message)
        {
            // Arrange
            string originId = "origin1";
            
            _mockedClientController.Setup(mock => mock.IsHost()).Returns(false);

            Player player = new Player("henk", 0, 0, "#", inventoryDTO.UserId);
            Item item = ItemFactory.GetBandage();

            for(int i = 0; i < 3; i++)
            {
                player.Inventory.AddConsumableItem((Items.Consumables.Consumable)item);
            }

            _mockedWorldService.Setup(mock => mock.GetPlayer(inventoryDTO.UserId)).Returns(player);
            _mockedClientController.Setup(mock => mock.GetOriginId()).Returns(originId);

            // Act
            _sut.HandlePacket(packetDTO);

            // Assert
            _mockedMessageService.Verify(mock => mock.AddMessage(message), Times.Once);
        }
        
        class HandlesDropPacketCases : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                string originId = "origin1";
                
                //Player drops Helmet
                InventoryDTO inventoryDTO = new(originId, InventoryType.Drop, 0);
                string payload = JsonConvert.SerializeObject(inventoryDTO);

                PacketDTO packetDTO = new();
                packetDTO.Payload = payload;
                PacketHeaderDTO packetHeaderDTO = new PacketHeaderDTO();
                packetHeaderDTO.Target = "host";
                packetDTO.Header = packetHeaderDTO;
                yield return new object[] { packetDTO, inventoryDTO, "You dropped Bandana" };
                
                //Player drops Knife
                InventoryDTO inventoryDTO2 = new(originId, InventoryType.Drop, 2);
                string payload2 = JsonConvert.SerializeObject(inventoryDTO2);
                
                PacketDTO packetDTO2 = new();
                packetDTO2.Payload = payload2;
                PacketHeaderDTO packetHeaderDTO2 = new PacketHeaderDTO();
                packetHeaderDTO2.Target = "host";
                packetDTO2.Header = packetHeaderDTO2;
                yield return new object[] { packetDTO2, inventoryDTO2, "You dropped Knife" };
                
                //Player drops Armor
                InventoryDTO inventoryDTO3 = new(originId, InventoryType.Drop, 1);
                string payload3 = JsonConvert.SerializeObject(inventoryDTO3);
                
                PacketDTO packetDTO3 = new();
                packetDTO3.Payload = payload3;
                PacketHeaderDTO packetHeaderDTO3 = new PacketHeaderDTO();
                packetHeaderDTO3.Target = "host";
                packetDTO3.Header = packetHeaderDTO3;
                yield return new object[] { packetDTO3, inventoryDTO3, "This is not an item you can drop!" };
                
                //Player drops Consumable 1
                InventoryDTO inventoryDTO4 = new(originId, InventoryType.Drop, 3);
                string payload4 = JsonConvert.SerializeObject(inventoryDTO4);
                
                PacketDTO packetDTO4 = new();
                packetDTO4.Payload = payload4;
                PacketHeaderDTO packetHeaderDTO4 = new PacketHeaderDTO();
                packetHeaderDTO4.Target = "host";
                packetDTO4.Header = packetHeaderDTO4;
                yield return new object[] { packetDTO4, inventoryDTO4, "You dropped Bandage" };
                
                //Player drops Consumable 2
                InventoryDTO inventoryDTO5 = new(originId, InventoryType.Drop, 4);
                string payload5 = JsonConvert.SerializeObject(inventoryDTO5);
                
                PacketDTO packetDTO5 = new();
                packetDTO5.Payload = payload5;
                PacketHeaderDTO packetHeaderDTO5 = new PacketHeaderDTO();
                packetHeaderDTO5.Target = "host";
                packetDTO5.Header = packetHeaderDTO5;
                yield return new object[] { packetDTO5, inventoryDTO5, "You dropped Bandage" };
                
                //Player drops Consumable 3
                InventoryDTO inventoryDTO6 = new(originId, InventoryType.Drop, 5);
                string payload6 = JsonConvert.SerializeObject(inventoryDTO6);
                
                PacketDTO packetDTO6 = new();
                packetDTO6.Payload = payload6;
                PacketHeaderDTO packetHeaderDTO6 = new PacketHeaderDTO();
                packetHeaderDTO6.Target = "host";
                packetDTO6.Header = packetHeaderDTO6;
                yield return new object[] { packetDTO6, inventoryDTO6, "You dropped Bandage" };
            }
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
        public void Test_HandlePacket_HandlesPickupPacket(InventoryDTO inventoryDTO, HandlerResponseDTO expectedHandlerResponseDTO, bool filledInventory, bool asHost, Item itemToAdd)
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
            items.Add(itemToAdd);
            if (filledInventory)
            {
                player.Inventory.AddConsumableItem(ItemFactory.GetBandage());
                player.Inventory.AddConsumableItem(ItemFactory.GetBandage());
                player.Inventory.AddConsumableItem(ItemFactory.GetBandage());
            }

            if (itemToAdd is Armor)
            {
                player.Inventory.Helmet = null;
            }

            _mockedWorldService.Setup(mock => mock.GetPlayer(inventoryDTO.UserId)).Returns(player);
            _mockedWorldService.Setup(mock => mock.GetItemsOnCurrentTile(player)).Returns(items);
            if (asHost)
            {
                _mockedClientController.Setup(mock => mock.IsHost()).Returns(true);
                _mockedClientController.Setup(mock => mock.GetOriginId()).Returns(inventoryDTO.UserId);
            }

            // Act
            HandlerResponseDTO handlerResponseDTO = _sut.HandlePacket(packetDTO);

            // Assert
            Assert.AreEqual(expectedHandlerResponseDTO, handlerResponseDTO);
            if (handlerResponseDTO.Action == SendAction.SendToClients)
            {
                _mockedPlayerItemServicesDb.Verify(mock => mock.CreateAsync(It.IsAny<PlayerItemPOCO>()), Times.Once());
            }
        }

        class HandlesPickupPacketCases : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                // Happy path
                yield return new object[]
                {
                    new InventoryDTO("userid", InventoryType.Pickup, 0),
                    new HandlerResponseDTO(SendAction.SendToClients, null),
                    false,
                    true,
                    ItemFactory.GetBandana()
                };
                // Happy path
                yield return new object[]
                {
                    new InventoryDTO("userid", InventoryType.Pickup, 0),
                    new HandlerResponseDTO(SendAction.SendToClients, null),
                    false,
                    true,
                    ItemFactory.GetBandage()
                };
                // Pickup item that is not in search list from client.
                yield return new object[]
                {
                    new InventoryDTO("userid", InventoryType.Pickup, 100),
                    new HandlerResponseDTO(SendAction.ReturnToSender, "Number is not in search list!"),
                    false,
                    false,
                    ItemFactory.GetBandage()
                };
                // Pickup item that is not in search list on host.
                yield return new object[]
                {
                    new InventoryDTO("userid", InventoryType.Pickup, 100),
                    new HandlerResponseDTO(SendAction.ReturnToSender, "Number is not in search list!"),
                    false,
                    true,
                    ItemFactory.GetBandage()
                };
                // Pickup consumable item with a full inventory from client.
                yield return new object[]
                {
                    new InventoryDTO("userid", InventoryType.Pickup, 0),
                    new HandlerResponseDTO(SendAction.ReturnToSender, "You already have 3 consumable items in your inventory!"),
                    true,
                    false,
                    ItemFactory.GetBandage()
                };
                // Pickup consumable item with a full inventory on host.
                yield return new object[]
                {
                    new InventoryDTO("userid", InventoryType.Pickup, 0),
                    new HandlerResponseDTO(SendAction.ReturnToSender, "You already have 3 consumable items in your inventory!"),
                    true,
                    true,
                    ItemFactory.GetBandage()
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
