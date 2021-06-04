﻿using System.Diagnostics.CodeAnalysis;

namespace ASD_project.ActionHandling.DTO
{
    [ExcludeFromCodeCoverage]
    public class MoveDTO
    {
        public string UserId { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }

        public MoveDTO(string userId, int xPosition, int yPosition)
        {
            UserId = userId;
            XPosition = xPosition;
            YPosition = yPosition;
        }
    }
}