using System;
using System.Diagnostics.CodeAnalysis;

namespace InputCommandHandler.Antlr.Ast.Actions
{
    public class Attack : Command, IEquatable<Attack>
    {
        private Direction _direction;
        public Direction Direction { get => _direction; private set => _direction = value; }
        // public Direction Direction { get; private set; }

        public override ASTNode AddChild(ASTNode child)
        {
            if (child is Direction)
            {
                _direction = (Direction) child;
            }

            return this;
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            return Equals(obj as Attack);
        }

        [ExcludeFromCodeCoverage]
        public bool Equals(Attack other)
        {
            if (other == null)
            {
                return false;
            }

            return _direction.Equals(other._direction);
        }
    }
}