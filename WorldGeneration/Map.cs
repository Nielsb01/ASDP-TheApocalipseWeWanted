﻿using System;
using System.Collections.Generic;
using System.Linq;
using WorldGeneration.Models;
using WorldGeneration.Models.Interfaces;

namespace WorldGeneration
{
    public class Map
    {
        private readonly int _chunkSize;
        private readonly int _seed;
        private List<Chunk> _chunks; // idk why it says this can be readonly, in line 33 i add chunks and in line 49 i remove chunks. NOT readonly
        private readonly Database.Database _db;
        private List<int[]> _chunksWithinLoadingRange;

        public Map(int chunkSize = 10, int seed = 0620520399, string dbLocation = "C:\\Temp\\ChunkDatabase.db", string dbCollectionName = "Chunks")
        {
            _chunkSize = chunkSize;
            _seed = seed;
            _db = new Database.Database(dbLocation, dbCollectionName);
            _chunks = new List<Chunk>();
        }

        // checks if there are new chunks that have to be loaded
        private void LoadArea(int playerX, int playerY, int viewDistance)
        {
            _chunksWithinLoadingRange = CalculateChunksToLoad(playerX, playerY, viewDistance);
            ForgetUnloadedChunks();
            foreach (var chunkXY in _chunksWithinLoadingRange)
            {
                if (!_chunks.Exists(chunk => chunk.X == chunkXY[0] && chunk.Y == chunkXY[1]))
                { // chunk isn't loaded in local memory yet
                    var chunk = _db.GetChunk(chunkXY[0], chunkXY[1]);
                    _chunks.Add(chunk == null
                        ? GenerateNewChunk(chunkXY[0], chunkXY[1])
                        : _db.GetChunk(chunkXY[0], chunkXY[1]));
                }
            }
        }

        // cleanup function to forget chunks out of loading range
        private void ForgetUnloadedChunks()
        {
            foreach (var loadedChunk in _chunks)
            {
                if (!_chunksWithinLoadingRange.Exists(
                    chunkWithinLoadingRange =>
                        chunkWithinLoadingRange[0] == loadedChunk.X
                        && chunkWithinLoadingRange[1] == loadedChunk.Y))
                {
                    if (!_chunks.Remove(loadedChunk))
                    {
                        throw new Exception("Failed to remove chunk from loaded chunks");
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

        public void DisplayMap(int playerX, int playerY, int viewDistance)
        {
            LoadArea(playerX, playerY, viewDistance);
            for (var y = (playerY + viewDistance); y > ((playerY + viewDistance) - (viewDistance * 2)); y--)
            {
                for (var x = (playerX - viewDistance); x < ((playerX - viewDistance) + (viewDistance * 2)); x++)
                {
                    var chunk = GetChunkForTileXAndY(x, y);
                    if (x == 0 && y == 0)
                    {
                        Console.Write(" " + '0'); // mark world center
                    }
                    else
                    {
                        Console.Write(" " + chunk.Map[chunk.GetPositionInTileArrayByWorldCoordinates(x, y)].Symbol);
                    }
                }
                Console.WriteLine("");
            }
        }

        private Chunk GenerateNewChunk(int chunkX, int chunkY)
        {
            var chunk = NoiseMapGenerator.GenerateChunk(chunkX, chunkY, _chunkSize, _seed);
            _db.InsertChunkIntoDatabase(chunk);
            return chunk;
        }

        private Chunk GetChunkForTileXAndY(int x, int y)
        {
            var chunk = _chunks.FirstOrDefault(chunk =>
                chunk.X * _chunkSize <= x 
                && chunk.X * _chunkSize > x - _chunkSize 
                && chunk.Y * _chunkSize >= y &&
                chunk.Y * _chunkSize < y + _chunkSize);
            
            if (chunk == null)
            {
                throw new Exception("Tried to find a chunk that has not been loaded");
            }
            return chunk;
        }

        public void DeleteMap()
        {
            _db.DeleteTileMap();
        }
        
        // find a LOADED tile by the coordinates
        public ITile GetLoadedTileByXAndY(int x, int y)
        {
            return GetChunkForTileXAndY(x, y).GetTileByWorldCoordinates(x, y);
        }
    }
}