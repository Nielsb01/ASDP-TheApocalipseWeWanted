﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ASD_Game.Items;
using ASD_Game.World.Models.Interfaces;

namespace ASD_Game.World.Models.BuildingTiles
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
            ItemsOnTile = new();
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