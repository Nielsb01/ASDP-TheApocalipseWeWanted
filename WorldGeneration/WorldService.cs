using WorldGeneration.Models.Interfaces;

namespace WorldGeneration
{
    public class WorldService : IWorldService
    {
        private World _world;

        public void UpdateCharacterPosition(string userId, int newXPosition, int newYPosition)
        {
            _world.UpdateCharacterPosition(userId, newXPosition, newYPosition);
        }

        public void UpdatePlayer(Player player)
        {
            _world.CurrentPlayer = player;
        }

        public void UpdateCharacterHealth(int health)
        {
            _world.UpdateCharacterHealth(health);
        }

        public void UpdateCharacterStamina(int stamina)
        {
            _world.UpdateCharacterStamina(stamina);
        }

        public void UpdateCharacterRadiationLevel(int radiationLevel)
        {
            _world.UpdateCharacterRadiationLevel(radiationLevel);
        }

        public void AddPlayerToWorld(Player player, bool isCurrentPlayer)
        {
            _world.AddPlayerToWorld(player, isCurrentPlayer);
        }

        public void DisplayWorld()
        {
            _world.DisplayWorld();
        }
        
        public void DeleteMap()
        {
            _world.deleteMap();
        }

        public void GenerateWorld(int seed)
        {
            _world = new World(seed, 6);
        }

        public Player getCurrentPlayer()
        {
            return _world.CurrentPlayer;
        }

        public ITile GetTile(int x, int y)
        {
            return _world.GetLoadedTileByXAndY(x, y);
        }
    }
}