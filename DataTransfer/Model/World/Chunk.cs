using System;
using DataTransfer.Model.World.Interfaces;

namespace DataTransfer.Model.World
{
    public class Chunk : IEquatable<Chunk>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ITile[] Map { get; set; }
        public int RowSize { get; set; }
        
        public Chunk()
        {
        }
        public Chunk(int x, int y, ITile[] map, int rowSize)
        {
            X = x;
            Y = y;
            Map = map;
            RowSize = rowSize;
        }

        //returns the coordinates relative to the start (left top) of the chunk. 0,0 is the left top. First value is x, second is y.
        public int[] GetTileCoordinatesInChunk(int indexInArray)
        {
            var x = indexInArray % RowSize;
            var y = (int) Math.Floor((double) indexInArray / RowSize);
            return new[] {x, y};
        }

        // returns the coordinates relative to the center of the world. First value is x, second is y.
        private int[] GetTileCoordinatesInWorld(int indexInArray)
        {
            var internalCoordinates = GetTileCoordinatesInChunk(indexInArray);
            return new[] {internalCoordinates[0] + RowSize * X, internalCoordinates[1] + RowSize * Y};
        }
        
        
        public int GetPositionInTileArrayByWorldCoordinates(int x, int y)
        {
            
            var yPos = Math.Abs(y);
            var chunkYPos = Math.Abs(Y);
            while (x < 0)
            {
                x = x + RowSize;
            }
            var y1 = (RowSize * RowSize - RowSize) -  (Math.Abs(chunkYPos * RowSize - yPos) * RowSize);
            var x1 = x % RowSize;
            
            return x1 + y1;
        }
        
        public ITile GetTileByWorldCoordinates(int x, int y)
        {
            return Map[GetPositionInTileArrayByWorldCoordinates(x,y)];
        }

        public bool Equals(Chunk other)
        {
            if (ReferenceEquals(null, other)) 
                return false;
            
            if (ReferenceEquals(this, other)) 
                return true;
            
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            
            if (ReferenceEquals(this, obj)) 
                return true;
            
            if (obj.GetType() != GetType()) 
                return false;
            
            return Equals((Chunk) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, RowSize);
        }
    }
}