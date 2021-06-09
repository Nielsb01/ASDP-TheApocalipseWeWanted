using ActionHandling.DTO;
using World.Models.Characters;
using WorldGeneration;

namespace ActionHandling
{
    public interface IRelativeStatHandler
    {
        public void CheckStaminaTimer();
        public void CheckRadiationTimer();
        public void SendStat(RelativeStatDTO stat);
        public void SetCurrentPlayer(Player player);
    }
}