using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ASD_Game.ActionHandling.DTO;
using ASD_Game.Items.Services;
using ASD_Game.UserInterface;
using ASD_Game.World.Models.Characters;
using ASD_Game.World.Models.Interfaces;

namespace ASD_Game.World
{
    public class World : IWorld
    {
        private Map _map;
        public Player CurrentPlayer { get; set; }
        public List<Player> Players { get; set; }
        public List<Character> _creatures { get; set; }
        public List<Character> movesList = new List<Character>();
        public List<ItemSpawnDTO> Items;
        
        private readonly int _viewDistance;
        private readonly IScreenHandler _screenHandler;
        private static readonly char _separator = Path.DirectorySeparatorChar;

        public World(int seed, int viewDistance, IMapFactory mapFactory, IScreenHandler screenHandler)
        {
            Players = new();
            _creatures = new();
            var currentDirectory = Directory.GetCurrentDirectory();

            Players = new();
            _map = MapFactory.GenerateMap(dbLocation: $"Filename={currentDirectory}{_separator}ChunkDatabase.db;connection=shared;", seed: seed);
            _viewDistance = viewDistance;
            _screenHandler = screenHandler;
            DeleteMap();
        }
        
        public Player GetPlayer(string id)
        {
            return Players.Find(x => x.Id == id);
        }

        public void UpdateCharacterPosition(string userId, int newXPosition, int newYPosition)
        {
            if (CurrentPlayer.Id == userId)
            {
                CurrentPlayer.XPosition = newXPosition;
                CurrentPlayer.YPosition = newYPosition;
            }
            else if (GetPlayer(userId) != null)
            {
                var player = GetPlayer(userId);
                player.XPosition = newXPosition;
                player.YPosition = newYPosition;
            }
            var creature = _creatures.FirstOrDefault(x => x.Id == userId);
            if (GetAI(userId) != null)
            {
                creature.XPosition = newXPosition;
                creature.YPosition = newYPosition;
            }
        }

        public void AddPlayerToWorld(Player player, bool isCurrentPlayer = false)
        {
            if (isCurrentPlayer)
            {
                CurrentPlayer = player;
            }
            Players.Add(player);
        }

        public void AddCreatureToWorld(Character character)
        {
            _creatures.Add(character);
        }

        public void UpdateMap()
        {
            if (CurrentPlayer != null && Players != null && _creatures != null)
            {
                _screenHandler.UpdateWorld(_map.GetMapAroundCharacter(CurrentPlayer, _viewDistance, GetAllCharacters()));
            }
        }

        public void DeleteMap()
        {
            _map.DeleteMap();
        }

        public void AddItemToWorld(ItemSpawnDTO itemSpawnDto)
        {
            Items.Add(itemSpawnDto);
            UpdateMap();
        }
        public ITile GetLoadedTileByXAndY(int x, int y)
        {
            return _map.GetLoadedTileByXAndY(x, y);
        }

        public bool CheckIfCharacterOnTile(ITile tile)
        {
            return GetAllCharacters().Exists(player => player.XPosition == tile.XPosition && player.YPosition == tile.YPosition);
        }

        public char[,] GetMapAroundCharacter(Character character)
        {
            return _map.GetMapAroundCharacter(character, _viewDistance, GetAllCharacters());
        }

        private List<Character> GetAllCharacters()
        {
            List<Character> characters = Players.Cast<Character>().ToList();
            characters.AddRange(_creatures);
            return characters;
        }

        public void LoadArea(int playerX, int playerY, int viewDistance)
        {
            _map.LoadArea(playerX, playerY, viewDistance);
        }

        public void UpdateAI()
        {
            movesList = new List<Character>();
            foreach (Character monster in _creatures)
            {
                if (monster is SmartMonster smartMonster)
                {
                    if (smartMonster.Brain != null)
                    {
                        UpdateSmartMonster(smartMonster);
                    }
                }
            }
        }

        private void UpdateSmartMonster(SmartMonster smartMonster)
        {
            smartMonster.Update();
            movesList.Add(smartMonster);
        }

        public Character GetAI(string id)
        {
            return _creatures.Find(x => x.Id == id);
        }

        public ITile GetCurrentTile()
        {
            return _map.GetLoadedTileByXAndY(CurrentPlayer.XPosition, CurrentPlayer.YPosition);
        }

        public ITile GetTileForPlayer(Player player)
        {
            return _map.GetLoadedTileByXAndY(player.XPosition, player.YPosition);
        }

        public List<Player> GetAllPlayers()
        {
            return Players;
        }
    }
}