using System;
using DataTransfer.DTO.Character;

namespace DataTransfer.DTO.Creature
{
    public class CreatureDTO : CharacterDTO
    {
        public CreatureDTO(int xPosition, int yPosition, string symbol, string playerGuid, ConsoleColor color = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black, int team = 0, int health = default, int stamina = default, int radiationLevel = default, InventoryDTO inventory = null, BitcoinDTO bitcoins = null) : base(xPosition, yPosition, symbol, playerGuid, color, backgroundColor, team, health, stamina, radiationLevel, inventory, bitcoins)
        {
        }
    }
}