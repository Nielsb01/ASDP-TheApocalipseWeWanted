﻿using System;
using System.Diagnostics.CodeAnalysis;
using LiteDB;

namespace ASD_project.DatabaseHandler.POCO
{
    [ExcludeFromCodeCoverage]
    public class AgentPoco
    {
        [BsonId] public string FileName { get; set; }

        public Guid PlayerGUID { get; set; }
        public Guid GameGUID { get; set; }
    }
}