﻿
namespace Player.Model
{
    public interface ICharacter
    {
        string Symbol { get; set; }
        int XPosition { get; set; }
        int YPosition { get; set; }
    }
}
