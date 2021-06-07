﻿using System.Diagnostics.CodeAnalysis;
using LiteDB;

namespace ASD_Game.DatabaseHandler.POCO
{
    [ExcludeFromCodeCoverage]
    public class ItemPoco
    {

        [BsonId]
        public string ItemName { get; set; }
        public int Xposition { get; set; }
        public int Yposition { get; set; }
    }
}
