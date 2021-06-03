﻿using System.Collections.Generic;
using System.Linq;
using ActionHandling;
using ActionHandling.DTO;
using Items;
using UserInterface;

namespace WorldGeneration
{
    public class World : IWorld
    {
        private IMap _map;
        public Player CurrentPlayer;
        public List<Player> _players;
        public List<ItemSpawnDTO> _items;
        public List<Creature> _creatures;
        private readonly int _viewDistance;
        private IScreenHandler _screenHandler;
        private IItemService _itemService;

        public World(int seed, int viewDistance, IMapFactory mapFactory, IScreenHandler screenHandler, IItemService itemService)
        {
            _items = new();
            _players = new ();
            _creatures = new ();
            _itemService = itemService;
            _map = mapFactory.GenerateMap(itemService, seed);
            _viewDistance = viewDistance;
            _screenHandler = screenHandler;
        }

        public void UpdateCharacterPosition(string id, int newXPosition, int newYPosition)
        {
            var player = _players.FirstOrDefault(x => x.Id == id);
            if (player != null)
            {
                player.XPosition = newXPosition;
                player.YPosition = newYPosition;
            }
            var creature = _creatures.FirstOrDefault(x => x.Id == id);
            if (creature != null)
            {
                creature.XPosition = newXPosition;
                creature.YPosition = newYPosition;
            }
            UpdateMap();
        }

        public void AddPlayerToWorld(Player player, bool isCurrentPlayer = false)
        {
            if (isCurrentPlayer)
            {
                CurrentPlayer = player;
            }
            _players.Add(player);
            UpdateMap();
        }
        
        public void AddCreatureToWorld(Creature creature)
        {
            _creatures.Add(creature);
            UpdateMap();
        }

        public void UpdateMap()
        {
            if (CurrentPlayer != null && _players != null && _creatures != null)
            {
                var characters = ((IEnumerable<Character>)_players).Concat(_creatures).ToList();
                //_screenHandler.UpdateWorld(_map.GetMapAroundCharacter(CurrentPlayer, _viewDistance, characters));
            }
        }

        public char[,] GetMapAroundCharacter(Character character)
        {
            var characters = ((IEnumerable<Character>)_players).Concat(_creatures).ToList();
            return _map.GetMapAroundCharacter(character, _viewDistance, characters);
        }

            public void DeleteMap()
        {
            _map.DeleteMap();
        }

            public void AddItemToWorld(ItemSpawnDTO itemSpawnDto)
            {
                _items.Add(itemSpawnDto);
                UpdateMap();
            }
    }
}
     
