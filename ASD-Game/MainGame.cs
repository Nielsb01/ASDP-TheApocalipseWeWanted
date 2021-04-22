using Microsoft.Extensions.Logging;
using System;
using WorldGeneration;

namespace ASD_project
{
    partial class Program
    {
        public class MainGame : IMainGame
        {
            private readonly ILogger<MainGame> log;

            public MainGame(ILogger<MainGame> log)
            {
                this.log = log;
            }

            public void Run()
            {
                new WorldGeneration.Program();
                // Note this code is for testing purposes only!
                Console.WriteLine("Game is gestart");
                WorldGenerationPrototype prototypeGenerator = new WorldGenerationPrototype(0);
                prototypeGenerator.GenerateThreeWorldsWithDifferentFrequencyButTheSameSeed(12345);
                prototypeGenerator.GenerateThreeWorldsWithDifferentFrequencyButTheSameSeed(987654);
            }
        }
    }
}
