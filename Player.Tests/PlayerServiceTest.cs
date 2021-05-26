﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Chat;
using DataTransfer.DTO.Character;
using DataTransfer.DTO.Player;
using Moq;
using Network;
using NUnit.Framework;
using Player.ActionHandlers;
using Player.Model;
using Player.Services;
using Session;
using WorldGeneration;

namespace Player.Tests
{
    [ExcludeFromCodeCoverage]
    public class PlayerServiceTest
    {
        private PlayerService _sut;
        private Mock<IPlayerModel> _mockedPlayerModel;
        private Mock<IMoveHandler> _mockedMoveHandler;
        private Mock<IChatHandler> _mockedChatHandler;
        private Mock<IClientController> _mockedClientController;
        private Mock<IWorldService> _mockedWorldService;
        private Mock<MapCharacterDTO> _mockedMapCharacterDTO;
        private Mock<SessionHandler> _mockedSessionHandler;

        [SetUp]
        public void Setup()
        {
            _mockedPlayerModel = new Mock<IPlayerModel>();
            _mockedMoveHandler = new Mock<IMoveHandler>();
            _mockedChatHandler = new Mock<IChatHandler>();
            _mockedClientController = new Mock<IClientController>();
            _mockedWorldService = new Mock<IWorldService>();

            _sut = new PlayerService(_mockedPlayerModel.Object, _mockedChatHandler.Object, _mockedMoveHandler.Object,
                _mockedClientController.Object, _mockedWorldService.Object);
        }

        [Test]
        public void Test_AddHealth_CallsFunctionFromPlayerModel()
        {
            int health = 10;
            _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.AddHealth(health));

            _sut.AddHealth(health);

            _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.AddHealth(health));
        }

        [Test]
        public void Test_RemoveHealth_CallsFunctionFromPlayerModel()
        {
            int health = 10;
            _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.RemoveHealth(health));

            _sut.RemoveHealth(health);

            _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.RemoveHealth(health));
        }

        [Test]
        public void Test_AddStamina_CallsFunctionFromPlayerModel()
        {
            int stamina = 10;
            _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.AddStamina(stamina));

            _sut.AddStamina(stamina);

            _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.AddStamina(stamina));
        }

        [Test]
        public void Test_RemoveStamina_CallsFunctionFromPlayerModel()
        {
            int stamina = 10;
            _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.RemoveStamina(stamina));

            _sut.RemoveStamina(stamina);

            _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.RemoveStamina(stamina));
        }

        [Test]
        public void Test_GetItem_CallsFunctionFromPlayerModel()
        {
            Item item = new Item("ItemName", "Description");
            _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.GetItem("ItemName")).Returns(item);

            Assert.AreEqual(item, _sut.GetItem("ItemName"));
            _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.GetItem("ItemName"));
        }

        [Test]
        public void Test_AddInventoryItem_CallsFunctionFromPlayerModel()
        {
            Item item = new Item("ItemName", "Description");
            _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.AddInventoryItem(item));

            _sut.AddInventoryItem(item);

            _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.AddInventoryItem(item));
        }

        [Test]
        public void Test_RemoveInventoryItem_CallsFunctionFromPlayerModel()
        {
            Item item = new Item("ItemName", "Description");
            _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.RemoveInventoryItem(item));

            _sut.RemoveInventoryItem(item);

            _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.RemoveInventoryItem(item));
        }

        [Test]
        public void Test_EmptyInventory_CallsFunctionFromPlayerModel()
        {
            _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.EmptyInventory());

            _sut.EmptyInventory();

            _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.EmptyInventory());
        }

        [Test]
        public void Test_AddBitcoins_CallsFunctionFromPlayerModel()
        {
            int amount = 10;
            _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.AddBitcoins(amount));

            _sut.AddBitcoins(amount);

            _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.AddBitcoins(amount));
        }

        [Test]
        public void Test_RemoveBitcoins_CallsFunctionFromPlayerModel()
        {
            int amount = 10;
            _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.RemoveBitcoins(amount));

            _sut.RemoveBitcoins(amount);

            _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.RemoveBitcoins(amount));
        }

        [Test]
        public void Test_GetAttackDamage_CallsFunctionFromPlayerModel()
        {
            int dmg = 5;
            _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.GetAttackDamage()).Returns(dmg);

            Assert.AreEqual(dmg, _sut.GetAttackDamage());
            _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.GetAttackDamage());
        }

        [Test]
        public void Test_PickupItem_CallsFunctionFromPlayerModel()
        {
            _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.PickupItem());

            _sut.PickupItem();

            _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.PickupItem());
        }

        [Test]
        public void Test_DropItem_CallsFunctionFromPlayerModel()
        {
            string itemName = "Test";
            _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.DropItem(itemName));

            _sut.DropItem(itemName);

            _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.DropItem(itemName));
        }

        // [Test]
        // public void Test_GetCurrentPosition_VerifyPlayerModelMoqWorks1()
        // {
        //     var direction_right = "right";
        //     int steps = 5;
        //     int x = 26;
        //     int y = 11;
        //     _mockedMapCharacterDTO = new Mock<MapCharacterDTO>(x,y, "test", "test2");
        //
        //     _mockedMapCharacterDTO.SetupSet(mock => mock.XPosition = x);
        //     _mockedMapCharacterDTO.SetupSet(mock => mock.YPosition = y);
        //     // _mockedMapCharacterDTO.Setup(_mockedMapCharacterDTO => _mockedMapCharacterDTO.YPosition >> y);
        //
        //     _sut.HandleDirection(direction_right, steps);
        //     
        //     // _mockedMapCharacterDTO.Verify(mock => mock.XPosition == x);
        //     // _mockedMapCharacterDTO.Verify(mock => mock.YPosition == y);
        //     _mockedMapCharacterDTO.Verify(_mockedMapCharacterDTO => _mockedMapCharacterDTO.XPosition, Times.Once);
        //     _mockedMapCharacterDTO.Verify(_mockedMapCharacterDTO => _mockedMapCharacterDTO.YPosition, Times.Once);
        //
        // }

        // [Test]
        // public void Test_GetCurrentPosition_VerifyPlayerModelMoqWorks2()
        // {
        //     var direction_left = "left";
        //     int steps = 5;
        //     int x = 26;
        //     int y = 11;
        //     
        //     _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.XPosition).Returns(x);
        //     _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.YPosition).Returns(y);
        //     
        //     _mockedMoveHandler.Setup(_mockedMoveHandler => _mockedMoveHandler.SendMove())
        //
        //     _sut.HandleDirection(direction_left, steps);
        //     
        //     _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.XPosition, Times.Once);
        //     _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.YPosition, Times.Once);
        // }
        //
        // [Test]
        // public void Test_GetCurrentPosition_VerifyPlayerModelMoqWorks3()
        // {
        //     var direction_left = "forward";
        //     int steps = 5;
        //     int x = 26;
        //     int y = 11;
        //     _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.SetNewPlayerPosition(0, -steps));
        //     _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.XPosition).Returns(x);
        //     _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.YPosition).Returns(y);
        //
        //     _sut.HandleDirection(direction_left, steps);
        //     
        //     _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.SetNewPlayerPosition(0, -steps), Times.Once);
        // }
        //
        // [Test]
        // public void Test_GetCurrentPosition_VerifyPlayerModelMoqWorks4()
        // {
        //     var direction_left = "backward";
        //     int steps = 5;
        //     int x = 26;
        //     int y = 11;
        //     _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.SetNewPlayerPosition(0, steps));
        //     _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.XPosition).Returns(x);
        //     _mockedPlayerModel.Setup(_mockedPlayerModel => _mockedPlayerModel.YPosition).Returns(y);
        //
        //     _sut.HandleDirection(direction_left, steps);
        //     
        //     _mockedPlayerModel.Verify(_mockedPlayerModel => _mockedPlayerModel.SetNewPlayerPosition(0, steps), Times.Once);
        // }
        //
        // [Test]
        // public void Test_Say_CallsFunctionFromChatHandler()
        // {
        //     // Arrange
        //     const string message = "hello world";
        //     _mockedChatHandler.Setup(mock => mock.SendSay(message));
        //
        //     // Act
        //     _sut.Say(message);
        //
        //     // Assert
        //     _mockedChatHandler.Verify(mock => mock.SendSay(message), Times.Once);
        // }

        // [Test]
        // public void Test_CreateSession_CallsFunctionFromSessionHandler()
        // {
        //     // Arrange
        //     const string sessionName = "cool world";
        //     _mockedSessionHandler.Setup(mock => mock.CreateSession(sessionName));
        //
        //     // Act
        //     _sut.CreateSession(sessionName);
        //
        //     // Assert
        //     _mockedSessionHandler.Verify(mock => mock.CreateSession(sessionName), Times.Once);
        // }
        //
        // [Test]
        // public void Test_RequestSessions_CallsFunctionFromSessionHandler()
        // {
        //     // Arrange
        //     _mockedSessionHandler.Setup(mock => mock.RequestSessions());
        //
        //     // Act
        //     _sut.RequestSessions();
        //     
        //     // Assert
        //     _mockedSessionHandler.Verify(mock => mock.RequestSessions(), Times.Once);
        // }
        //
        // [Test]
        // public void Test_JoinSession_CallsFunctionFromSessionHandler()
        // {
        //     // Arrange
        //     const string sessionId = "1234-1234";
        //     _mockedSessionHandler.Setup(mock => mock.JoinSession(sessionId));
        //     
        //     // Act
        //     _sut.JoinSession(sessionId);
        //     
        //     // Assert
        //     _mockedSessionHandler.Verify(mock => mock.JoinSession(sessionId));
        // }

        
//          [Test]
//         public void Test_RemoveHealth_WithoutDying()
//         {
//             _sut.RemoveHealth(50);
//             
//             Assert.AreEqual(50, _sut.Health);
//         }
//         
//         [Test]
//         public void Test_RemoveHealth_StopsAtDyingState()
//         {
//             _sut.RemoveHealth(200);
//             
//             Assert.AreEqual(0, _sut.Health);
//         }
//         
//         [Test]
//         public void Test_AddHealth_WithoutExceedingHealthCap()
//         {
//             _sut.RemoveHealth(50);
//             
//             _sut.AddHealth(40);
//             
//             Assert.AreEqual(90, _sut.Health);
//         }
//         
//         [Test]
//         public void Test_AddHealth_ReachesHealthCap()
//         {
//             _sut.RemoveHealth(30);
//             
//             _sut.AddHealth(40);
//             
//             Assert.AreEqual(100, _sut.Health);
//         }
//         
//         [Test]
//         public void Test_RemoveStamina_WithoutRunningOutOfMana()
//         {
//             _sut.RemoveStamina(5);
//             
//             Assert.AreEqual(5, _sut.Stamina);
//         }
//         
//         [Test]
//         public void Test_RemoveStamina_StopsAtNoMana()
//         {
//             _sut.RemoveStamina(20);
//             
//             Assert.AreEqual(0, _sut.Stamina);
//         }
//         
//         [Test]
//         public void Test_AddStamina_WithoutExceedingStaminaCap()
//         {
//             _sut.RemoveStamina(5);
//             
//             _sut.AddStamina(4);
//             
//             Assert.AreEqual(9, _sut.Stamina);
//         }
//         
//         [Test]
//         public void Test_AddStamina_ReachesStaminaCap()
//         {
//             _sut.RemoveStamina(3);
//             
//             _sut.AddStamina(4);
//             
//             Assert.AreEqual(10, _sut.Stamina);
//         }
//         
//         [Test]
//         public void Test_GetItem_VerifyInventoryMoqWorks()
//         {
//             Item item = new Item("ItemName", "Description");
//             _mockedInventory.Setup(mockedInventory => mockedInventory.GetItem("ItemName")).Returns(item);
//             
//             Assert.AreEqual(item, _sut.GetItem("ItemName"));
//             _mockedInventory.Verify(mockedInventory => mockedInventory.GetItem("ItemName"), Times.Once);
//         }
//         
//         [Test]
//         public void Test_AddInventoryItem_AddsItemSuccessfully()
//         {
//             Item item = new Item("ItemName", "Description");
//             _mockedInventory.Setup(mockedInventory => mockedInventory.AddItem(item));
//
//             _sut.AddInventoryItem(item);
//             
//             _mockedInventory.Verify(mockedInventory => mockedInventory.AddItem(item), Times.Once);
//         }
//         
//         [Test]
//         public void Test_RemoveInventoryItem_RemovesItemSuccessfully()
//         {
//             Item item = new Item("ItemName", "Description");
//             _mockedInventory.Setup(mockedInventory => mockedInventory.RemoveItem(item));
//
//             _sut.RemoveInventoryItem(item);
//             
//             _mockedInventory.Verify(mockedInventory => mockedInventory.RemoveItem(item), Times.Once);
//         }
//         
//         [Test]
//         public void Test_EmptyInventory_EmptiesInventorySuccessfully()
//         {
//             _mockedInventory.Setup(mockedInventory => mockedInventory.EmptyInventory());
//
//             _sut.EmptyInventory();
//             
//             _mockedInventory.Verify(mockedInventory => mockedInventory.EmptyInventory(), Times.Once);
//         }
//         
//         [Test]
//         public void Test_GetAmount_VerifyBitcoinMoqWorks()
//         {
//             _mockedBitcoins.Setup(mockedBitcoins => mockedBitcoins.Amount).Returns(20);
//
//             Assert.AreEqual(20, _sut.Bitcoins.Amount);
//             _mockedBitcoins.Verify(mockedBitcoins => mockedBitcoins.Amount, Times.Once);
//         }
//         
//         [Test]
//         public void Test_AddBitcoins_AddsBitcoinsSuccessfully()
//         {
//             _mockedBitcoins.Setup(mockedBitcoins => mockedBitcoins.AddAmount(20));
//
//             _sut.AddBitcoins(20);
//             
//             _mockedBitcoins.Verify(mockedBitcoins => mockedBitcoins.AddAmount(20), Times.Once);
//         }
//         
//         [Test]
//         public void Test_RemoveBitcoins_RemovesBitcoinsSuccessfully()
//         {
//             _mockedBitcoins.Setup(mockedBitcoins => mockedBitcoins.RemoveAmount(10));
//
//             _sut.RemoveBitcoins(10);
//             
//             _mockedBitcoins.Verify(mockedBitcoins => mockedBitcoins.RemoveAmount(10), Times.Once);
//         }
//         
//         [Test]
//         public void Test_DropItem_DropsItemSuccessfully()
//         {
//             Item item = new Item("ItemName", "Description");
//             _mockedInventory.Setup(mockedInventory => mockedInventory.GetItem("ItemName")).Returns(item);
//             _mockedInventory.Setup(mockedInventory => mockedInventory.RemoveItem(item));
//
//             _sut.DropItem("ItemName");
//             
//             _mockedInventory.Verify(mockedInventory => mockedInventory.RemoveItem(item), Times.Once);
//         }
//         
//         [Test]
//         public void Test_DropItem_ThrowsExceptionBecauseNoItemExists()
//         {
//             _mockedInventory.Setup(mockedInventory => mockedInventory.GetItem("ItemName"));
//         
//             Assert.Throws<ItemException>(() => _sut.DropItem("ItemName"));
//         }
//         
//         [Test]
//         public void Test_GetAttackDamage_GetDefaultAttackDamage()
//         {
//             Assert.AreEqual(5, _sut.GetAttackDamage());
//         }
//         
//         [Test]
//         public void Test_GetLevel_VerifyRadiationLevelMoqWorks()
//         {
//             _mockedRadiationLevel.Setup(mockedRadiationLevel => mockedRadiationLevel.Level).Returns(1);
//
//             Assert.AreEqual(1, _sut.RadiationLevel.Level);
//             _mockedRadiationLevel.Verify(mockedRadiationLevel => mockedRadiationLevel.Level, Times.Once);
//         }
//
    }
}
