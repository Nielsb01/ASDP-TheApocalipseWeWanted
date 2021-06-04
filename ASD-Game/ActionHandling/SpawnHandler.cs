﻿using System.Collections.Generic;
using System.Linq;
using ASD_project.ActionHandling.DTO;
using ASD_project.DatabaseHandler.POCO;
using ASD_project.DatabaseHandler.Services;
using ASD_project.Items;
using ASD_project.Network;
using ASD_project.Network.DTO;
using ASD_project.Network.Enum;
using Newtonsoft.Json;

namespace ASD_project.ActionHandling
{
    public class SpawnHandler : ISpawnHandler, IPacketHandler
    {
        private IClientController _clientController;
        private List<ItemSpawnDTO> _itemSpawnDTOs;

        public SpawnHandler(IClientController clientController)
        {
            _clientController = clientController;
            _clientController.SubscribeToPacketType(this, PacketType.Spawn);
        }

        public void SetItemSpawnDtOs(List<ItemSpawnDTO> itemSpawnDTOs)
        {
            _itemSpawnDTOs = itemSpawnDTOs;
        }

        public void SendSpawn(int x, int y, Item item)
        {
            ItemSpawnDTO itemSpawnDto = new ItemSpawnDTO();
            itemSpawnDto.XPosition = x;
            itemSpawnDto.YPosition = y;
            itemSpawnDto.Item = item;
            SendSpawnDTO(itemSpawnDto);
        }

        private void SendSpawnDTO(ItemSpawnDTO itemSpawnDto)
        {
            var payload = JsonConvert.SerializeObject(itemSpawnDto);
            _clientController.SendPayload(payload, PacketType.Spawn);
        }

        public HandlerResponseDTO HandlePacket(PacketDTO packet)
        {
            var itemSpawnDto = JsonConvert.DeserializeObject<ItemSpawnDTO>(packet.Payload);
            
            if (_clientController.IsHost() && packet.Header.Target.Equals("host"))
            {
                ItemSpawnDTO item = _itemSpawnDTOs
                    .FirstOrDefault(item => item.Equals(itemSpawnDto));
                if (item == null)
                {
                    InsertToDatabase(itemSpawnDto);
                }
                else
                {
                    return new HandlerResponseDTO(SendAction.Ignore, null);
                }
            }
            return new HandlerResponseDTO(SendAction.SendToClients, null);
        }

        private void InsertToDatabase(ItemSpawnDTO itemSpawnDto)
        {
            var ItemService = new DatabaseService<ItemPoco>();
            var item = new ItemPoco()
                {ItemName = itemSpawnDto.Item.ItemName, Xposition = itemSpawnDto.XPosition, Yposition = itemSpawnDto.YPosition};
            ItemService.CreateAsync(item);
        }
    }
}