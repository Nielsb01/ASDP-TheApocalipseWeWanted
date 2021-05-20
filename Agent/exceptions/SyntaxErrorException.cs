﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace Agent.exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class SyntaxErrorException : Exception
    {
        public SyntaxErrorException(string message) : base(message)
        {
    
        }
    }
}