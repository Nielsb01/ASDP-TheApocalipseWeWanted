﻿using System.Collections.Generic;

namespace Player.Model
{
    public class Inventory : IInventory
    {
        private List<IItem> _consumableItems;
        public List<IItem> ConsumableItemList { get => _consumableItems; set => _consumableItems = value; }

        private IItem _armor;
        public IItem Armor { get => _armor; set => _armor = value; }

        private IItem _helmet;
        public IItem Helmet { get => _helmet; set => _helmet = value; }

        private IItem _meleeWeapon;
        public IItem MeleeWeapon { get => _meleeWeapon; set => _meleeWeapon = value; }

        private IItem _rangedWeapon;
        public IItem RangedWeapon { get => _rangedWeapon; set => _rangedWeapon = value; }

        public Inventory()
        {
            _consumableItems = new List<IItem>();
        }

        public IItem GetItem(string itemName)
        {
            foreach (var item in _consumableItems)
            {
                if (item.ItemName == itemName)
                {
                    return item;
                }
            }
            return null;
        }

        public void AddItem(IItem item)
        {
            _consumableItems.Add(item);
        }

        public void RemoveItem(IItem item)
        {
            _consumableItems.Remove(item);
        }

        public void EmptyInventory()
        {
            _consumableItems.Clear();
        }
    }
}
