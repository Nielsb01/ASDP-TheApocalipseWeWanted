﻿using Items;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WorldGeneration.Models.Interfaces;

namespace WorldGeneration.Models.BuildingTiles
{
    [ExcludeFromCodeCoverage]
    public class HouseTile : IBuildingTile
    {
        public bool IsAccessible { get; set; }
        public string Symbol { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public int StaminaCost { get; set; }
        public List<Item> ItemsOnTile { get; set; }

        public HouseTile()
        {
            Symbol = TileSymbol.HOUSE;
            IsAccessible = true;
            StaminaCost = 1;
        }

        public void DrawBuilding()
        {
            throw new NotImplementedException();
        }
    }
}