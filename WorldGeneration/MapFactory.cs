using System;
using DatabaseHandler.Services;
using DataTransfer.POCO.World;

namespace WorldGeneration
{
    public abstract class MapFactory
    {
        public static Map GenerateMap(int chunkSize = 8, int seed = -1123581321)
        {
            // default chunksize is 8. Can be adjusted in the line above
            
            // seed can be null, if it is it becomes random. But because of how c# works you can't set a default null, so this workaround exists.
            if (seed == -1123581321)
            {
                seed = new Random().Next(1, 999999);
            }           
            return new Map(new NoiseMapGenerator(), chunkSize, seed, new ServicesDb<Chunk>());
        }

        public static int GenerateSeed()
        {
            var randomSeed = new Random().Next(1, 999999);
            return randomSeed;
        }
    }
}