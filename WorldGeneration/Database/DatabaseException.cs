#nullable enable
using System;

namespace WorldGeneration.Database
{
    public class DatabaseException : Exception
    {
        public DatabaseException()
        {
        }
        public DatabaseException(string? message) : base(message)
        {
        }
    }
}