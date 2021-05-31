using System;
using System.Diagnostics.CodeAnalysis;
using DatabaseHandler.Services;
using DataTransfer.Model.World;
using Display;

namespace WorldGeneration
{
    public class MapFactory: IMapFactory

    {
        [ExcludeFromCodeCoverage]
        
        public IMap GenerateMap(int seed = 0)
        {
            return GenerateMap(8, seed);
            // default chunksize is 8. Can be adjusted in the line above
        }
        
        [ExcludeFromCodeCoverage]
        public IMap GenerateMap(int chunkSize, int seed)
        {
            // If seed is 0 it becomes random
            if (seed == 0)
            {
                seed = GenerateSeed();
            }

            return new Map(new NoiseMapGenerator(), chunkSize, seed, new ConsolePrinter(), new DatabaseService<Chunk>());
        }

        public int GenerateSeed()
        {
            return new Random().Next(1, 9999999);
        }
    }
}