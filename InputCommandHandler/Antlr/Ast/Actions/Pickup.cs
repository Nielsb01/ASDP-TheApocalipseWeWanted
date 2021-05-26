using System;
using System.Diagnostics.CodeAnalysis;

namespace InputCommandHandler.Antlr.Ast.Actions
{
    public class Pickup : Command, IEquatable<Pickup>
    {
        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            return Equals(obj as Pickup);
        }

        [ExcludeFromCodeCoverage]
        public bool Equals(Pickup other)
        {
            return true;
        }
    }
}