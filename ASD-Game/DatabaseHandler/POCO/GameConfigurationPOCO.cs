using LiteDB;
using System.Diagnostics.CodeAnalysis;

namespace DatabaseHandler.POCO
{
    [ExcludeFromCodeCoverage]
    public class GameConfigurationPOCO
    {
        [BsonId]
        public string GameGUID { get; set; }

        public int NPCDifficultyCurrent { get; set; }
        public int NPCDifficultyNew { get; set; }
        public int ItemSpawnRate { get; set; }
    }
}