﻿using System.Diagnostics.CodeAnalysis;
using WorldGeneration.Models.Interfaces;

namespace WorldGeneration.Models.TerrainTiles
{
    [ExcludeFromCodeCoverage]
    public class StreetTile : ITerrainTile
    {
        
        public bool IsAccessible { get; set; }
        public string Symbol { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public StreetTile(int x, int y)
        {
            Symbol = TileSymbol.STREET;
            IsAccessible = true;
            XPosition = x;
            YPosition = y;
        }
    }
}