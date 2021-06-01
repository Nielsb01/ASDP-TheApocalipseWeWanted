﻿using System;
using System.Collections.Generic;

namespace WorldGeneration
{
    public class World
    {
        private Map _map;
        public Player CurrentPlayer { get; set; }
        private List<Player> _players;
        private string currentIdPlayer = null; 
        private readonly int _viewDistance;

        public World(int seed, int viewDistance)
        {
            _players = new ();
            _map = MapFactory.GenerateMap(seed: seed);
            _viewDistance = viewDistance;
            _map.DeleteMap();
        }

        public void UpdateCharacterPosition(string userId, int newXPosition, int newYPosition)
        {
            if (CurrentPlayer == null)
            {
                foreach (var element in _players)
                {
                    if (element.Id.Equals(userId) && element.Id.Equals(currentIdPlayer))
                    {
                        CurrentPlayer = element;
                    }
                }

                Console.WriteLine(CurrentPlayer);
                if (CurrentPlayer.Id.Equals(userId))
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
            }
        DisplayWorld();
        }

        public void AddExistingPlayerToWorld(Player player, bool isCurrentPlayer)
        {
            if (isCurrentPlayer)
            {
                CurrentPlayer = player;
                currentIdPlayer = player.Id;
            }
            _players.Add(player);
        }

        public void AddPlayerToWorld(Player player, bool isCurrentPlayer)
        {
            if (isCurrentPlayer)
            {
                CurrentPlayer = player;
                currentIdPlayer = player.Id;
            }
            _players.Add(player);
        }

        public void DisplayWorld()
        {
            if (CurrentPlayer != null && _players != null)
            {
                Console.Clear();
                _map.DisplayMap(CurrentPlayer, _viewDistance, new List<Character>(_players));
            }
            
        }

        public void deleteMap()
        {
            _map.DeleteMap();
        }

        public void SetCurrentPlayerClient(string playerId)
        {
            foreach (var element in _players)
            {
                if (element.Id.Equals(playerId))
                {
                    CurrentPlayer = element;
                    _players.Add(element);
                }
            }
        }

        public void SetCurrentPlayerHost(string playerId)
        {
            foreach (var element in _players)
            {
                if (element.Id.Equals(playerId))
                {
                    CurrentPlayer = element;
                    _players.Add(element);
                }
            }
        }
    }
}
     
