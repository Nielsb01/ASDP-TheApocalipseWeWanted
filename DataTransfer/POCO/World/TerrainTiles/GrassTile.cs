﻿using DataTransfer.POCO.World.Interfaces;

namespace DataTransfer.POCO.World.TerrainTiles
{
    public class GrassTile : ITerrainTile
    {
        
        public bool IsAccessible { get; set; }
        public string Symbol { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public GrassTile(int x, int y)
        {
            Symbol = TileSymbol.GRASS;
            IsAccessible = true;
            XPosition = x;
            YPosition = y;
        }
    }
}