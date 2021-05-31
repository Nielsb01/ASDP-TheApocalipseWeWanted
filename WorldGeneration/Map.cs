﻿using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseHandler;
using DatabaseHandler.Repository;
using DatabaseHandler.Services;
using Display;
using DataTransfer.DTO.Character;
using DataTransfer.Model.World;
using DataTransfer.Model.World.Interfaces;
using Microsoft.Extensions.Primitives;
using WorldGeneration.Services;

namespace WorldGeneration
{
    public class Map : IMap
    {
        private readonly int _chunkSize;
        private readonly int _seed;
        private IList<Chunk> _chunks;
        private IDatabaseService<Chunk> _dbService;
        private ChunkService _chunkService;
        private IList<int[]> _chunksWithinLoadingRange;

        private INoiseMapGenerator _noiseMapGenerator;
        private IConsolePrinter _consolePrinter;

        public Map(
            INoiseMapGenerator noiseMapGenerator
            , int chunkSize
            , int seed
            , IDatabaseService<Chunk> dbServices
            , IConsolePrinter consolePrinter
            , IList<Chunk> chunks = null
        )
        {
            if (chunkSize < 1)
            {
                throw new InvalidOperationException("Chunksize smaller than 1.");
            }
            _chunkSize = chunkSize;
            _chunks = chunks ?? new List<Chunk>();
            _seed = seed;
            _noiseMapGenerator = noiseMapGenerator;
            _dbService = dbServices;
            _consolePrinter = consolePrinter;
        }

        // checks if there are new chunks that have to be loaded
        private void LoadArea(int playerX, int playerY, int viewDistance) {
            _chunksWithinLoadingRange = CalculateChunksToLoad(playerX, playerY, viewDistance);
            foreach (var chunkXY in _chunksWithinLoadingRange)
            {
                if (!_chunks.Any(chunk => chunk.X == chunkXY[0] && chunk.Y == chunkXY[1]))
                { // chunk isn't loaded in local memory yet
                    var chunk = new Chunk { 
                        X = chunkXY[0], 
                        Y = chunkXY[1] 
                    };
                    var getAllChunksQuery = _dbService.GetAllAsync();
                    getAllChunksQuery.Wait();
                    var results = getAllChunksQuery.Result.FirstOrDefault(c => c.X == chunkXY[0] && c.Y == chunkXY[1]);
                    if (results == null)
                    {
                        _chunks.Add(GenerateNewChunk(chunkXY[0], chunkXY[1]));
                    }
                    else
                    {
                        _chunks.Add(results);
                    }
                }
            }
        }

        private List<int[]> CalculateChunksToLoad(int playerX, int playerY, int viewDistance)
        {
            // viewDistance * 2 is to get a full screen
            // , + playerX to get to the right location
            // , + chunksize to add some loading buffer
            // , / chunksize to convert tile coordinates to world coordinates
            // same for the other variables
            var maxX = (playerX + viewDistance * 2 + _chunkSize) / _chunkSize; 
            var minX = (playerX - viewDistance * 2 - _chunkSize) / _chunkSize;
            var maxY = (playerY + viewDistance * 2 + _chunkSize) / _chunkSize + 1;
            var minY = (playerY - viewDistance * 2 - _chunkSize) / _chunkSize;
            var chunksWithinLoadingRange = new List<int[]>();

            for (var x = minX; x <= maxX; x++)
            {
                for (var y = minY; y < maxY; y++)
                {
                    chunksWithinLoadingRange.Add(new[] {x, y});
                }
            }
            return chunksWithinLoadingRange;
        }

        public void DisplayMap(MapCharacterDTO currentPlayer, int viewDistance, IList<MapCharacterDTO> characters)
        {
            if (viewDistance < 0)
            {
                throw new InvalidOperationException("viewDistance smaller than 0.");
            }
            
            var playerX = currentPlayer.XPosition;
            var playerY = currentPlayer.YPosition;
            LoadArea(playerX, playerY, viewDistance);
            for (var y = (playerY + viewDistance); y > ((playerY + viewDistance) - (viewDistance * 2) -1); y--)
            {
                for (var x = (playerX - viewDistance); x < ((playerX - viewDistance) + (viewDistance * 2) + 1); x++)
                {
                    var tile = GetLoadedTileByXAndY(x, y);
                    _consolePrinter.PrintText($"  {GetDisplaySymbol(currentPlayer, tile, characters)}");
                }
                _consolePrinter.NextLine();
            }
        }

        public char[,] GetMapAroundCharacter(MapCharacterDTO centerCharacter, int viewDistance, IList<MapCharacterDTO> allCharacters)
        {
            if (viewDistance < 0)
            {
                throw new InvalidOperationException("viewDistance smaller than 0.");
            }
            
            var tileArray = new char[viewDistance * 2 + 1, viewDistance * 2 + 1];
            var centerCharacterXPosition = centerCharacter.XPosition;
            var centerCharacterYPosition = centerCharacter.YPosition;
            LoadArea(centerCharacterXPosition, centerCharacterYPosition, viewDistance);
            
            for (var y = (centerCharacterYPosition + viewDistance); y > ((centerCharacterYPosition + viewDistance) - (viewDistance * 2) -1); y--)
            {
                for (var x = (centerCharacterXPosition - viewDistance); x < ((centerCharacterXPosition - viewDistance) + (viewDistance * 2) + 1); x++)
                {
                    var tile = GetLoadedTileByXAndY(x, y);
                    tileArray[x,y]
                }
                _consolePrinter.NextLine();
            }
            return tileArray;
        }
        
        private string GetDisplaySymbol(MapCharacterDTO currentPlayer, ITile tile, IList<MapCharacterDTO> characters)
        {
            bool currentPlayerOnTile = IsPlayerOnTile(tile, currentPlayer);
            if (currentPlayerOnTile)
            {
                return currentPlayer.Symbol;
            }
            foreach (var characterOnTile in characters.Where(character => character.XPosition == tile.XPosition && character.YPosition - 1 == tile.YPosition))
            {
                if (characterOnTile.Symbol == CharacterSymbol.FRIENDLY_PLAYER)
                {
                    if (characterOnTile.Team != currentPlayer.Team || characterOnTile.Team == 0)
                    {
                        return CharacterSymbol.ENEMY_PLAYER;
                    }
                    return CharacterSymbol.FRIENDLY_PLAYER;
                }
                return characterOnTile.Symbol;
            }
            return tile.Symbol;
        }

        private bool IsPlayerOnTile(ITile tile, MapCharacterDTO player)
        {
            return tile.XPosition == player.XPosition && tile.YPosition == player.YPosition - 1;
        }

        private Chunk GenerateNewChunk(int chunkX, int chunkY)
        {
            var chunk = _noiseMapGenerator.GenerateChunk(chunkX, chunkY, _chunkSize, _seed);
            _dbService.CreateAsync(chunk);
            return chunk;
        }

        private Chunk GetChunkForTileXAndY(int x, int y)
        {
            var chunk = _chunks.FirstOrDefault(chunk =>
                chunk.X * _chunkSize <= x 
                && chunk.X * _chunkSize > x - _chunkSize 
                && chunk.Y * _chunkSize >= y &&
                chunk.Y * _chunkSize < y + _chunkSize);
            return chunk;
        }
        
        public void DeleteMap()
        {
            _chunks.Clear();
            _dbService.DeleteAllAsync();
        }
        
        // find a LOADED tile by the coordinates
        public ITile GetLoadedTileByXAndY(int x, int y)
        {
            _chunkService = new ChunkService(GetChunkForTileXAndY(x, y));
            return _chunkService.GetTileByWorldCoordinates(x, y);
        }
    }
}