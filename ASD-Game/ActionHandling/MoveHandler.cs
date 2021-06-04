﻿using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using ActionHandling.DTO;
using DatabaseHandler.POCO;
using DatabaseHandler.Services;
using Messages;
using Network.DTO;
using Newtonsoft.Json;
using WorldGeneration;
using WorldGeneration.Models.Interfaces;

namespace ActionHandling
{
    public class MoveHandler : IMoveHandler, IPacketHandler
    {
        private readonly IClientController _clientController;
        private readonly IWorldService _worldService;
        private readonly IDatabaseService<PlayerPOCO> _playerServicesDB;
        private readonly IMessageService _messageService;

        public MoveHandler(IClientController clientController, IWorldService worldService, IDatabaseService<PlayerPOCO> playerServicesDb, IMessageService messageService)
        {
            _clientController = clientController;
            _clientController.SubscribeToPacketType(this, PacketType.Move);
            _worldService = worldService;
            _playerServicesDB = playerServicesDb;
            _messageService = messageService;
        }

        public void SendMove(string directionValue, int stepsValue)
        {
            int x = 0;
            int y = 0;

            switch (directionValue)
            {
                case "right":
                case "east":
                    x = stepsValue;
                    break;
                case "left":
                case "west":
                    x = -stepsValue;
                    break;
                case "forward":
                case "up":
                case "north":
                    y = +stepsValue;
                    break;
                case "backward":
                case "down":
                case "south":
                    y = -stepsValue;
                    break;
            }

            var currentPlayer = _worldService.GetCurrentPlayer();
            MoveDTO moveDTO = new(currentPlayer.Id, currentPlayer.XPosition + x, currentPlayer.YPosition + y);

            SendMoveDTO(moveDTO);
        }

        private void SendMoveDTO(MoveDTO moveDTO)
        {
            var payload = JsonConvert.SerializeObject(moveDTO);
            _clientController.SendPayload(payload, PacketType.Move);
        }

        public HandlerResponseDTO HandlePacket(PacketDTO packet)
        {
            var moveDTO = JsonConvert.DeserializeObject<MoveDTO>(packet.Payload);
            bool handleInDatabase = (_clientController.IsHost() && packet.Header.Target.Equals("host")) || _clientController.IsBackupHost;

            if (packet.Header.Target == "host" || packet.Header.Target == "client")
            {
                return HandleMove(moveDTO, handleInDatabase);
            }
            else
            {
                _messageService.AddMessage(packet.HandlerResponse.ResultMessage);
            }
            return new(SendAction.Ignore, null);
        }

        public HandlerResponseDTO HandleMove(MoveDTO moveDTO, bool handleInDatabase)
        {
            var player = _worldService.GetPlayer(moveDTO.UserId);
            
            var newPosPlayerX = moveDTO.XPosition;
            var newPosPlayerY = moveDTO.YPosition;
            var oldPosPlayerX = player.XPosition;
            var oldPosPlayerY = player.YPosition;
            var playerStamina = player.Stamina;
            
            _worldService.LoadArea(newPosPlayerX, newPosPlayerY, 10);
            var allTiles = GetTilesForPositions(oldPosPlayerX, oldPosPlayerY, newPosPlayerX, newPosPlayerY);
            var accessibleTiles = inAccessibleTileExists(allTiles);
            var movableTiles = GetMovableTiles(allTiles, accessibleTiles);
            var staminaCost = GetStaminaCostForTiles(movableTiles);
            
            if (staminaCost > playerStamina)
            {
                moveDTO = ChangeMoveDTOToNewLocation(moveDTO, movableTiles, player);
                
                if (moveDTO.UserId == _clientController.GetOriginId())
                {
                    _messageService.AddMessage("You do not have enough stamina to move!");
                }
                
                //edit payload

                return new HandlerResponseDTO(SendAction.ReturnToSender, "You do not have enough stamina to move!");
            }
            else
            {
                string resultMessage = null;
                moveDTO.Stamina = playerStamina - staminaCost;
                //allTiles.Count-1 because the maximum count movableTiles always has 1 less tile, since it doesn't contain the first tile
                if (movableTiles.Count < allTiles.Count - 1)
                {
                    moveDTO = ChangeMoveDTOToNewLocation(moveDTO, movableTiles, player);
                    resultMessage = "You could not move to the next tile.";

                    if (moveDTO.UserId == _clientController.GetOriginId())
                    {
                        _messageService.AddMessage(resultMessage);
                    }
                }

                ChangePlayerPosition(moveDTO);

                if (handleInDatabase)
                {
                    InsertToDatabase(moveDTO);
                }
                
                //edit payload
                // packet.Payload = JsonConvert.SerializeObject(moveDTO);
                
                return new HandlerResponseDTO(SendAction.SendToClients, resultMessage);
            }
        }

        private void InsertToDatabase(MoveDTO moveDTO)
        {
            var player = _playerServicesDB.GetAllAsync().Result.FirstOrDefault(player => player.PlayerGuid == moveDTO.UserId && player.GameGuid == _clientController.SessionId);
            player.XPosition = moveDTO.XPosition;
            player.YPosition = moveDTO.YPosition;
            player.Stamina = moveDTO.Stamina;
            _playerServicesDB.UpdateAsync(player);
        }

        private MoveDTO ChangeMoveDTOToNewLocation(MoveDTO moveDTO, List<ITile> movableTiles, Player player)
        {
            if (movableTiles.Count != 0)
            {
                moveDTO.XPosition = movableTiles.Last().XPosition;
                moveDTO.YPosition = movableTiles.Last().YPosition;
            }
            else
            {
                moveDTO.XPosition = player.XPosition;
                moveDTO.YPosition = player.YPosition;
            }

            return moveDTO;
        }

        private void ChangePlayerPosition(MoveDTO moveDTO)
        {
            var player = _worldService.GetPlayer(moveDTO.UserId);
            player.Stamina = moveDTO.Stamina;
            player.XPosition = moveDTO.XPosition;
            player.YPosition = moveDTO.YPosition;
            //display stats worldservice
            _worldService.DisplayWorld();
        }

        private List<ITile> GetTilesForPositions(int x1, int y1, int x2, int y2)
        {
            var tiles = new List<ITile>();
 
            if (y1 != y2)
            {
                if (y2 > y1)
                {
                    for (int i = y1; i <= y2; i++)
                    {
                        tiles.Add(_worldService.GetTile(x1, i));
                    }
                }
                else
                {
                    for (int i = y1; i >= y2; i--)
                    {
                        tiles.Add(_worldService.GetTile(x1, i));
                    }
                }
            }
            else
            {
                if (x2 > x1)
                {
                    for (int i = x1; i <= x2; i++)
                    {
                        tiles.Add(_worldService.GetTile(i, y1));
                    }
                }
                else
                {
                    for (int i = x1; i >= x2; i--)
                    {
                        tiles.Add(_worldService.GetTile(i, y1));
                    }
                }
            }
            return tiles;
        }

        private int GetStaminaCostForTiles(List<ITile> tiles)
        {
            var staminaCosts = 0;
            
            foreach (var tile in tiles)
            {
                staminaCosts += tile.StaminaCost;
            }
            
            return staminaCosts;
        }

        private List<bool> inAccessibleTileExists(List<ITile> tiles)
        {
            var accessibleTiles = new List<bool>();
            //i=1 because we want to skip the first tile, since the player is standing on that tile and it doesn't matter if that tile is accessible.
            for (int i = 1; i < tiles.Count; i++)
            {
                accessibleTiles.Add(tiles[i].IsAccessible);
            }
            return accessibleTiles;
        }

        private List<ITile> GetMovableTiles(List<ITile> tiles, List<bool> accessible)
        {
            var movableTiles = new List<ITile>();
            for (int i = 0; i < accessible.Count; i++)
            {
                //tiles[i+1] since we don't want to check the first tile, since the player is standing on that tile and the accessible list has 1 tile less than list tiles
                if (accessible[i] && !_worldService.CheckIfPlayerOnTile(tiles[i+1]))
                {
                    movableTiles.Add(tiles[i+1]);
                }
                else
                {
                    break;
                }
            }
            return movableTiles;
        }
    }
}