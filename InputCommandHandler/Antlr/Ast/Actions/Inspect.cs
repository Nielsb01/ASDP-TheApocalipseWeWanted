using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace InputCommandHandler.Antlr.Ast.Actions
{
    public class Inspect: Command
    {
        private InventorySlot _inventorySlot;
        [ExcludeFromCodeCoverage]
        public InventorySlot InventorySlot { get => _inventorySlot; private set => _inventorySlot = value; }

        [ExcludeFromCodeCoverage]
        public ArrayList GetChildren()
        {
            var children = new ArrayList();
            children.Add(_inventorySlot);
            return children;
        }

        public override ASTNode AddChild(ASTNode child)
        {
            if (child is InventorySlot)
            {
                _inventorySlot = (InventorySlot) child;
            }

            return this;
        }

        [ExcludeFromCodeCoverage]
        public ASTNode RemoveChild(ASTNode child)
        {
            if (child is InventorySlot && child == _inventorySlot)
            {
                _inventorySlot = null;
            }

            return this;
        }  
    }
}