using System;
using System.Diagnostics.CodeAnalysis;
using LiteDB;

namespace DatabaseHandler.POCO
{
    [ExcludeFromCodeCoverage]
    public class PlayerItemPOCO
    {
        public string PlayerGUID { get; set; }
        public string GameGUID { get; set; }
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int ArmorPoints { get; set; }
    }
}
