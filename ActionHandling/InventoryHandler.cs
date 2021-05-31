﻿using Network;
using Network.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldGeneration;

namespace ActionHandling
{
    public class InventoryHandler : IPacketHandler, IInventoryHandler
    {
        private IClientController _clientController;
        private IWorldService _worldService;

        public InventoryHandler(IClientController clientController, IWorldService worldService)
        {
            _clientController = clientController;
            _clientController.SubscribeToPacketType(this, PacketType.Inventory);
            _worldService = worldService;
        }

        public void Search()
        {
            string searchResult = _worldService.SearchCurrentTile();
        }

        public void DropItem(string inventorySlot)
        {
            switch (inventorySlot)
            {
                case "armor":
                    _worldService.DropItemOnTile(_worldService.getCurrentPlayer().Inventory.Armor);
                    _worldService.getCurrentPlayer().Inventory.Armor = null;
                    break;
                case "helmet":
                    _worldService.DropItemOnTile(_worldService.getCurrentPlayer().Inventory.Helmet);
                    _worldService.getCurrentPlayer().Inventory.Helmet = null;
                    break;
                case "weapon":
                    _worldService.DropItemOnTile(_worldService.getCurrentPlayer().Inventory.Weapon);
                    _worldService.getCurrentPlayer().Inventory.Weapon = null;
                    break;
                case "slot 1":
                    _worldService.DropItemOnTile(_worldService.getCurrentPlayer().Inventory.ConsumableItemList[1]);
                    _worldService.getCurrentPlayer().Inventory.RemoveConsumableItem(_worldService.getCurrentPlayer().Inventory.ConsumableItemList[1] = null);
                    break;
                case "slot 2":
                    _worldService.DropItemOnTile(_worldService.getCurrentPlayer().Inventory.GetConsumableItem(inventorySlot));
                    _worldService.DropItemOnTile(_worldService.getCurrentPlayer().Inventory.ConsumableItemList[2]);
                    _worldService.getCurrentPlayer().Inventory.RemoveConsumableItem(_worldService.getCurrentPlayer().Inventory.ConsumableItemList[2] = null);
                    break;
                case "slot 3":
                    _worldService.DropItemOnTile(_worldService.getCurrentPlayer().Inventory.ConsumableItemList[3]);
                    _worldService.getCurrentPlayer().Inventory.RemoveConsumableItem(_worldService.getCurrentPlayer().Inventory.ConsumableItemList[3] = null);
                    break;
                default:
                    Console.WriteLine("Unknown inventory slot");
                    break;
            }
        }



        public HandlerResponseDTO HandlePacket(PacketDTO packet)
        {
            throw new NotImplementedException();
        }
    }
}
