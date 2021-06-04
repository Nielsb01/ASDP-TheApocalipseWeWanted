using System.Diagnostics.CodeAnalysis;
using LiteDB;

namespace ASD_project.DatabaseHandler.POCO
{
    [ExcludeFromCodeCoverage]
    public class PlayerPOCO
    {
        public string GameGUID { get; set; }
        [BsonId]
        public string PlayerGUID { get; set; }
        public string PlayerName { get; set; }
        public int TypePlayer { get; set; }
        public int Health { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
    }
}