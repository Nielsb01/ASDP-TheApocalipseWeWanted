﻿using System;
using System.Collections.Generic;
using System.Linq;
using UserInterface;

namespace WorldGeneration
{
    public class World : IWorld
    {
        private IMap _map;
        public Player CurrentPlayer { get; set; }
        private List<Player> _players;
        private readonly int _viewDistance;
        private IScreenHandler _screenHandler;

        public World(int seed, int viewDistance, IMapFactory mapFactory, IScreenHandler screenHandler)
        {
            _players = new ();
            _map = mapFactory.GenerateMap(seed);
            _viewDistance = viewDistance;
            _screenHandler = screenHandler;
            DeleteMap();
        }

        public void UpdateCharacterPosition(string userId, int newXPosition, int newYPosition)
        {
            if (CurrentPlayer.Id == userId)
            {
                CurrentPlayer.XPosition = newXPosition;
                CurrentPlayer.YPosition = newYPosition;
            }
            else
            {
                var player = _players.Find(x => x.Id == userId);
                player.XPosition = newXPosition;
                player.YPosition = newYPosition;
            }
            UpdateMapInConsole();
        }

        public void AddPlayerToWorld(Player player, bool isCurrentPlayer)
        {
            if (isCurrentPlayer)
            {
                CurrentPlayer = player;
            }
            _players.Add(player);
        }

        public void DisplayWorld()
        {
            if (CurrentPlayer != null && _players != null)
            {
                UpdateMapInConsole();
            }
        }

        public void DeleteMap()
        {
            _map.DeleteMap();
        }

        private void UpdateMapInConsole()
        {
            _screenHandler.UpdateWorld(_map.GetMapAroundCharacter(CurrentPlayer, _viewDistance, _players));
        }
    }
}
     
