﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ASD_Game.Items;
using ASD_Game.World.Models.Interfaces;

namespace ASD_Game.World.Models.BuildingTiles
{
    [ExcludeFromCodeCoverage]
    public class DoorTile : IBuildingTile
    {
        public bool IsAccessible { get; set; }
        public string Symbol { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public int StaminaCost { get; set; }
        public List<Item> ItemsOnTile { get; set; }

        public DoorTile()
        {
            ItemsOnTile = new();
            Symbol = TileSymbol.DOOR;
            IsAccessible = true;
        }

        public void DrawBuilding()
        {
            throw new NotImplementedException();
        }
    }
}