using System;

namespace DataTransfer.DTO.Character
{
    public class MapCharacterDTO
    {
        public int XPosition;
        public int YPosition;
        public string PlayerGuid;
        public string GameGuid;
        public string Symbol;
        public ConsoleColor Color;
        public ConsoleColor BackgroundColor;
        public int Team;

        public MapCharacterDTO(int xPosition
            , int yPosition
            , string playerGuid
            , string gameGuid
            , string symbol = null
            , ConsoleColor color = ConsoleColor.White
            , ConsoleColor backgroundColor = ConsoleColor.Black
            , int team = 0)
        {
            XPosition = xPosition;
            YPosition = yPosition;
            PlayerGuid = playerGuid;
            GameGuid = gameGuid;
            Symbol = symbol;
            Color = color;
            BackgroundColor = backgroundColor;
            Team = team;
        }
    }
}