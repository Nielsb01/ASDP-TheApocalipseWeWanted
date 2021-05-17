﻿using WorldGeneration.Models;
using WorldGeneration.Models.HazardousTiles;
using WorldGeneration.Models.Interfaces;
using WorldGeneration.Models.TerrainTiles;

namespace WorldGeneration
{
    public class NoiseMapGenerator : INoiseMapGenerator
    {
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
            var noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            noise.SetSeed(seed);
            noise.SetFrequency(0.03f);
            noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            var map = new ITile[chunkRowSize * chunkRowSize];
            for (var y = 0; y < chunkRowSize; y++)
            {
                for (var x = 0; x < chunkRowSize; x++)
                {
                    map[y * chunkRowSize + x] = GetTileFromNoise(noise.GetNoise(x + chunkX * chunkRowSize, y + chunkY * chunkRowSize));
                }
            }
            return new Chunk(chunkX, chunkY, map, chunkRowSize);
        }

        private ITile GetTileFromNoise(float noise)
        {
            return (noise * 10) switch
            {
                (< -8) => new WaterTile(),
                (< -4) => new DirtTile(),
                (< 2) => new GrassTile(),
                (< 3) => new SpikeTile(),
                (< 8) => new StreetTile(),
                _ => new GasTile()
            };
        }
    }
}