﻿using WorldGeneration.Models;
using WorldGeneration.Models.Enums;
using WorldGeneration.Models.HazardousTiles;
using WorldGeneration.Models.Interfaces;
using WorldGeneration.Models.TerrainTiles;

namespace WorldGeneration
{
    public class NoiseMapGenerator : INoiseMapGenerator
    {
        private FastNoiseLite _noise;
        public int[,] GenerateAreaMap(int size, int seed)
        {
            var noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            noise.SetSeed(seed);
            noise.SetFrequency(0.015f);
            noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            var noiseData = new int[size, size];
            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
                noiseData[x, y] = (int) noise.GetNoise(x, y);
            return noiseData;
        }

        public Chunk GenerateChunk(int chunkX, int chunkY, int chunkRowSize, int seed)
        {
            if (_noise == null)
            {
                SetupNoise(seed);
            }
            
            // check which edges of the chunk will have roads.
            CompassDirections compassDirection = GetCompassDirections(chunkX, chunkY);
            
            // Create a map that has the roads, but these roads are missing coordinates.
            // var roadmap = RoadPresetFactory.GetRoadPreset(chunkRowSize, compassDirection);
            var map = new ITile[chunkRowSize * chunkRowSize];
            var roadmap = RoadPresetFactory.GetRoadPreset(chunkRowSize, CompassDirections.NorthToSouth);
            for (var y = 0; y < chunkRowSize; y++)
            {
                for (var x = 0; x < chunkRowSize; x++)
                {
                    if (roadmap[y * chunkRowSize + x] != null)
                    {
                       map[y * chunkRowSize + x] = new StreetTile(x, y);
                    }
                    else
                    {
                        // If there isn't a tile here yet, give it one.
                        map[y * chunkRowSize + x] = GetTileFromNoise(
                            _noise.GetNoise(x + chunkX * chunkRowSize, y + chunkY * chunkRowSize)
                            , x + chunkRowSize * chunkX
                            , chunkRowSize * chunkY - chunkRowSize + y);
                    }
                }
            }
            return new Chunk(chunkX, chunkY, map, chunkRowSize);
        }

        private CompassDirections GetCompassDirections (int chunkX, int chunkY)
        {
            // Step 1: generate four binaries based on the compass directions.
            // These use the coordinate of the current chunk and the adjacent chunk, so when the calculation is repeated in the adjacent chunk it will result in the same result.
            int north = GetBinaryForRoads(chunkY + chunkY + 1);
            int south = GetBinaryForRoads(chunkY + chunkY - 1);
            int east = GetBinaryForRoads(chunkX + chunkX + 1);
            int west = GetBinaryForRoads(chunkX + chunkX - 1);
            
            // Step 2: based on the previous binaries, a switch case is used to return the correct compass direction.
            switch (north, south, east, west)
            {
                case (0, 0, 0, 0):
                    return CompassDirections.NoRoads;
                case (1, 0, 0, 0):
                    return CompassDirections.NorthOnly;
                case (0, 1, 0, 0):
                    return CompassDirections.SouthOnly;
                case (0, 0, 1, 0):
                    return CompassDirections.EastOnly;
                case (0, 0, 0, 1):
                    return CompassDirections.WestOnly;
                default:
                    // Default solution, also used for roads not yet implemented.
                    return CompassDirections.NoRoads;
            }
        }

        private int GetBinaryForRoads(int combinedcoordinates)
        {
           float noiseresult = _noise.GetNoise(combinedcoordinates, combinedcoordinates);
           return (int) (noiseresult % 2);
        }


        private ITile GetTileFromNoise(float noiseResult, int x, int y)
        {
            return (noiseResult * 10) switch
            {
                (< -8) => new WaterTile(x, y),
                (< -4) => new DirtTile(x, y),
                (< 2) => new GrassTile(x, y),
                (< 3) => new SpikeTile(x, y),
                (< 8) => new DirtTile(x, y), // Replaces this with DirtTile because we don't want randomly generated street tiles.
                _ => new GasTile(x, y)
            };
        }

        private void SetupNoise(int seed)
        {
            _noise = new FastNoiseLite();
            _noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _noise.SetSeed(seed);
            _noise.SetFrequency(0.03f);
            _noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
        }
    }
}