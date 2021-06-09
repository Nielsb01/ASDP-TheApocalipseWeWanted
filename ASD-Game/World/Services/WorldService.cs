using System;
using System.Collections.Generic;
using ASD_Game.ActionHandling.DTO;
using ASD_Game.Items;
using ASD_Game.Items.Services;
using ASD_Game.UserInterface;
using ASD_Game.World.Models.Characters;
using ASD_Game.World.Models.Characters.Algorithms.NeuralNetworking;
using ASD_Game.World.Models.Interfaces;
using World.Models.Characters.Algorithms.NeuralNetworking;

namespace ASD_Game.World.Services
{
    public class WorldService : IWorldService
    {
        private readonly IItemService _itemService;
        private readonly IScreenHandler _screenHandler;
        public IWorld World { get; set; }
        public List<Character> CreatureMoves { get; set; }
        
        public WorldService(IScreenHandler screenHandler, IItemService itemService)
        {
            _screenHandler = screenHandler;
            _itemService = itemService;
        }

        public void UpdateCharacterPosition(string userId, int newXPosition, int newYPosition)
        {
            World.UpdateCharacterPosition(userId, newXPosition, newYPosition);
        }

        public void AddPlayerToWorld(Player player, bool isCurrentPlayer)
        {
            World.AddPlayerToWorld(player, isCurrentPlayer);
        }

        public void AddCreatureToWorld(Monster character)
        {
            World.AddCreatureToWorld(character);
        }

        public void DisplayWorld()
        {
            World.UpdateMap();
        }
        
        public void DeleteMap()
        {
            World.DeleteMap();
        }

        public void GenerateWorld(int seed)
        {
            World = new World(seed, 6, new MapFactory(), _screenHandler, _itemService);
        }

        public Player GetCurrentPlayer()
        {
            return World.CurrentPlayer;
        }

        public char[,] GetMapAroundCharacter(Character character)
        {
            return World.GetMapAroundCharacter(character);
        }

        public List<Monster> GetMonsters()
        {
            return World.Creatures;
        }

        public void UpdateBrains(Genome genome)
        {
            if (World != null)
            {
                foreach (Character monster in World.Creatures)
                {
                    if (monster is SmartMonster smartMonster)
                    {
                        smartMonster.Brain = genome;
                    }
                }
            }
        }

        public List<Character> GetCreatureMoves()
        {
            if (World != null)
            {
                World.UpdateAI();
                return World.MovesList;
            }
            return null;
        }

        public List<Player> GetAllPlayers()
        {
            return World.Players;
        }

        public bool IsDead(Player player)
        {
            return player.Health <= 0;
        }
        
        public void LoadArea(int playerX, int playerY, int viewDistance)
        {
            World.LoadArea(playerX, playerY, viewDistance);
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
            return World.GetPlayer(userId);
        }

        public Character GetAI(string id)
        {
            return World.GetAI(id);
        }

        public ITile GetTile(int x, int y)
        {
            return World.GetLoadedTileByXAndY(x, y);
        }

        public bool CheckIfCharacterOnTile(ITile tile)
        {
            return World.CheckIfCharacterOnTile(tile);
        }

        public void DisplayStats()
        {
            Player player = GetCurrentPlayer();
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
        
        public IList<Item> GetItemsOnCurrentTile()
        {
            return World.GetCurrentTile().ItemsOnTile;
        }
        
        public IList<Item> GetItemsOnCurrentTile(Player player)
        {
            return World.GetTileForPlayer(player).ItemsOnTile;
        }
    }
}