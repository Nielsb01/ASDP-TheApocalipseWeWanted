using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ASD_project.ActionHandling.DTO;
using ASD_project.Items;
using ASD_project.Items.Services;
using ASD_project.UserInterface;
using ASD_project.World.Models.Characters;
using ASD_project.World.Models.Interfaces;

namespace ASD_project.World.Services
{
    [ExcludeFromCodeCoverage]
    public class WorldService : IWorldService
    {
        private readonly IItemService _itemService;
        private readonly IScreenHandler _screenHandler;
        private World _world;

        public WorldService(IScreenHandler screenHandler, IItemService itemService)
        {
            _screenHandler = screenHandler;
            _itemService = itemService;
        }

        public void UpdateCharacterPosition(string userId, int newXPosition, int newYPosition)
        {
            _world.UpdateCharacterPosition(userId, newXPosition, newYPosition);
        }

        public void AddPlayerToWorld(Player player, bool isCurrentPlayer)
        {
            _world.AddPlayerToWorld(player, isCurrentPlayer);
        }

        public void DisplayWorld()
        {
            _world.UpdateMap();
        }

        public void DeleteMap()
        {
            _world.DeleteMap();
        }

        public void GenerateWorld(int seed)
        {
            _world = new World(seed, 6, new MapFactory(), _screenHandler, _itemService);
        }

        public Player getCurrentPlayer()
        {
            return _world.CurrentPlayer;
        }

        public List<ItemSpawnDTO> getAllItems()
        {
            return _world.Items;
        }

        public void AddItemToWorld(ItemSpawnDTO itemSpawnDTO)
        {
            _world.AddItemToWorld(itemSpawnDTO);
        }

        public Player GetCurrentPlayer()
        {
            return _world.CurrentPlayer;
        }


        public string SearchCurrentTile()
        {
            var itemsOnCurrentTile = GetItemsOnCurrentTile();


            var result = "The following items are on the current tile:" + Environment.NewLine;
            var index = 1;
            foreach (var item in itemsOnCurrentTile)
            {
                result += $"{index}. {item.ItemName}{Environment.NewLine}";
                index += 1;
            }

            return result;
        }


        public Player GetPlayer(string userId)
        {
            return _world.GetPlayer(userId);
        }


        public void DisplayStats()
        {
            var player = GetCurrentPlayer();
            _screenHandler.SetStatValues(
                player.Name,
                0,
                player.Health,
                player.Stamina,
                player.GetArmorPoints(),
                player.RadiationLevel,
                player.Inventory.Helmet?.ItemName ?? "Empty",
                player.Inventory.Armor?.ItemName ?? "Empty",
                player.Inventory.Weapon?.ItemName ?? "Empty",
                player.Inventory.GetConsumableAtIndex(0)?.ItemName ?? "Empty",
                player.Inventory.GetConsumableAtIndex(1)?.ItemName ?? "Empty",
                player.Inventory.GetConsumableAtIndex(2)?.ItemName ?? "Empty");
        }

        public World GetWorld()
        {
            return _world;
        }

        public char[,] GetMapAroundCharacter(Character character)
        {
            return _world.GetMapAroundCharacter(character);
        }


        public IList<Item> GetItemsOnCurrentTile()
        {
            return _world.GetCurrentTile().ItemsOnTile;
        }


        public IList<Item> GetItemsOnCurrentTile(Player player)
        {
            return _world.GetTileForPlayer(player).ItemsOnTile;
        }


        public ITile GetTile(int x, int y)
        {
            return _world.GetLoadedTileByXAndY(x, y);
        }

        public bool CheckIfCharacterOnTile(ITile tile)
        {
            return _world.CheckIfCharacterOnTile(tile);
        }


        public void LoadArea(int playerX, int playerY, int viewDistance)
        {
            _world.LoadArea(playerX, playerY, viewDistance);
        }


        public List<Player> GetPlayers()
        {
            return _world.GetAllPlayers();
        }
    }
}