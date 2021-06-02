﻿using System;
using System.Diagnostics.CodeAnalysis;
using Items.ArmorStats;
using Items.WeaponStats;
using LiteDB;

namespace DatabaseHandler.POCO
{
    [ExcludeFromCodeCoverage]
    public class PlayerItemPOCO
    {
        
        public string PlayerGUID { get; set; }

        public string GameGUID { get; set; }
        
        [BsonId]
        public string Id = Guid.NewGuid().ToString();
        public string ItemName { get; set; }

        public string ItemType { get; set; }

        public ArmorPartType ArmorPartType { get; set; }

        public int ArmorPoints { get; set; }
        
        public int Damage { get; set; }
    }
}