﻿using System.Collections.Generic;
using Items;
using Items.ArmorStats;
using Items.Consumables;

namespace WorldGeneration
{
    public class Inventory
    {
        private List<Consumable> _consumableItems;
        public List<Consumable> ConsumableItemList { get => _consumableItems; set => _consumableItems = value; }

        private Armor _armor;
        public Armor Armor { get => _armor; set => _armor = value; }

        private Armor _helmet;
        public Armor Helmet { get => _helmet; set => _helmet = value; }

        private Weapon _weapon;
        public Weapon Weapon { get => _weapon; set => _weapon = value; }

        public Inventory()
        {
            _consumableItems = new List<Consumable>();
            _helmet = ItemFactory.GetBandana();
            _weapon = ItemFactory.GetKnife();
        }

        public Consumable GetConsumableItem(string itemName)
        {
            return _consumableItems.Find(item => item.ItemName == itemName);
        }

        public bool AddConsumableItem(Consumable consumable)
        {
            if (_consumableItems.Count < 3)
            {
                _consumableItems.Add(consumable);
                return true;
            }
            else
            {
                System.Console.WriteLine("You already have 3 consumable items in your inventory!");
                return false;
            }
        }

        /// <summary>
        /// Returns false if item could not be added. True otherwise.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddItem(Item item)
        {
            if (item is Weapon weapon)
            {
                if (_weapon != null) return false;
                _weapon = weapon;
                return true;
            }

            if (item is Armor armor)
            {
                if (armor.ArmorPartType == ArmorPartType.Body)
                {
                    if (_armor != null) return false;
                    _armor = armor;
                    return true;
                }

                if (armor.ArmorPartType == ArmorPartType.Helmet)
                {
                    if (_helmet != null) return false;
                    _helmet = armor;
                    return true;
                }
            }

            if (item is Consumable consumable)
            {
                return AddConsumableItem(consumable);
            }

            return false;
        }

        public void RemoveConsumableItem(Consumable consumable)
        {
            _consumableItems.Remove(consumable);
        }

        public void EmptyConsumableItemList()
        {
            _consumableItems.Clear();
        }
    }
}
