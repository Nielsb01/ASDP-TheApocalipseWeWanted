using System;

namespace WorldGeneration
{
    public class Program
    {
        public Program()
        {
            
            var db = new Database.Database();
            db.DeleteTileMap();
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" En dan nu een random chunk met de seed " + 2243 + ": ");
            Console.ForegroundColor = ConsoleColor.Red;
            var map = new Map(120, 2243);
            map.LoadArea(new[] {0, 0}, 30);
            db.DeleteTileMap();
            Console.ForegroundColor = ConsoleColor.White;
            var seed = new Random().Next(999999);
            Console.WriteLine(" En dan nu een random chunk met de seed " + seed + ": ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            var randomMap = new Map(120, seed);
            randomMap.LoadArea(new[] {0, 0}, 30);
            
            
            
        }
    }
}