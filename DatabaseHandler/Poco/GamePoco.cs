using System;
using System.Diagnostics.CodeAnalysis;
using LiteDB;

namespace DatabaseHandler.Poco
{


    [ExcludeFromCodeCoverage]
    public class GamePoco
    {
        [BsonId]
        public string GameGuid { get; set; }
    //    public Guid PlayerGUIDHost { get; set; }
       public int Seed { get; set; }

    }
}