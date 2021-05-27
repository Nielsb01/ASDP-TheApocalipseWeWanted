﻿using Items;
using Items.Armor;
using Items.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGeneration
{
    public class Inventory
    {
        private List<Item> _consumableItems;
        public List<Item> ConsumableItemList { get => _consumableItems; set => _consumableItems = value; }

        private Item _armor;
        public Item Armor { get => _armor; set => _armor = value; }

        private Item _helmet;
        public Item Helmet { get => _helmet; set => _helmet = value; }

        private Item _weapon;
        public Item Weapon { get => _weapon; set => _weapon = value; }

        public Inventory()
        {
            _consumableItems = new List<Item>();
            //should make real items
            _helmet = new Armor();
            _weapon = new Weapon();
        }

        public Item GetConsumableItem(string itemName)
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

        public void AddConsumableItem(Item item)
        {
            if (_consumableItems.Count <= 3)
            {
                _consumableItems.Add(item);
            }
            else
            {
                System.Console.WriteLine("You already have 3 consumable items in your inventory!");
            }
        }

        public void RemoveConsumableItem(Item item)
        {
            _consumableItems.Remove(item);
        }

        public void EmptyConsumableItemList()
        {
            _consumableItems.Clear();
        }
    }
}
