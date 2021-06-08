﻿using ASD_Game.ActionHandling;
using ASD_Game.World;

namespace ASD_Game.Items.Services
{
    public class ItemService : IItemService
    {
        private ISpawnHandler _spawnHandler;
        public ItemService(ISpawnHandler spawnHandler)
        {
            _spawnHandler = spawnHandler;
        }
        
        public Item GenerateItemFromNoise(float noiseResult, int x, int y)
        {
            var item = RandomItemGenerator.GetRandomItem(noiseResult);
            if (item != null)
            {
                item.ItemId = (x + "!" + y + "!");
                _spawnHandler.SendSpawn(x, y, item);
            }
            return item;
        }

        public ISpawnHandler GetSpawnHandler()
        {
            return _spawnHandler;
        }
    }
}