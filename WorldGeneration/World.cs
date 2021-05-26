﻿using System;
using System.Collections.Generic;
using System.Linq;
using DataTransfer.DTO.Character;
using WorldGeneration.Models.Interfaces;

namespace WorldGeneration
{
    public class World
    {
        private Map _map;
        public MapCharacterDTO CurrentPlayer { get; set; }
        private IList<MapCharacterDTO> _characters { get; set; }
        private readonly int _viewDistance;

        public World(int seed, int viewDistance)
        {
            _characters = new List<MapCharacterDTO>();
            // AddCharacterToWorld(currentPlayer);
            _map = MapFactory.GenerateMap(seed: seed);
            _viewDistance = viewDistance;
            _map.DeleteMap();
        }

        public void UpdateCharacterPosition(MapCharacterDTO characterPositionDTO)
        {
            if (CurrentPlayer.PlayerGuid == characterPositionDTO.PlayerGuid)
            {
                CurrentPlayer.XPosition = characterPositionDTO.XPosition;
                CurrentPlayer.YPosition = characterPositionDTO.YPosition;
            }

            if (_characters.Any(x => x.PlayerGuid.Equals(characterPositionDTO.PlayerGuid)))
            {
                _characters.Where(x => x.PlayerGuid.Equals(characterPositionDTO.PlayerGuid)).FirstOrDefault().XPosition = characterPositionDTO.XPosition;
                _characters.Where(x => x.PlayerGuid.Equals(characterPositionDTO.PlayerGuid)).FirstOrDefault().YPosition = characterPositionDTO.YPosition;                
            }

            DisplayWorld();
        }

        public void AddCharacterToWorld(MapCharacterDTO mapCharacterDTO, bool isCurrentPlayer)
        {
            if (isCurrentPlayer)
            {
                CurrentPlayer = mapCharacterDTO;
            }
            
            _characters.Add(mapCharacterDTO);
        }

        public void DisplayWorld()
        {
            if (CurrentPlayer is null || _characters is null)
            {
                return;
            }
            Console.Clear();
            _map.DisplayMap(CurrentPlayer, _viewDistance, _characters);
        }

        public void deleteMap()
        {
            _map.DeleteMap();
        }

        public ITile GetCurrentTile()
        {
            return _map.GetLoadedTileByXAndY(CurrentPlayer.XPosition, CurrentPlayer.YPosition);
        }
    }
}
     
