using System.Diagnostics.CodeAnalysis;

namespace ActionHandling.DTO
{
    [ExcludeFromCodeCoverage]
    public class RelativeStatDTO
    {
        public string Id { get; set; }
        public int Health { get; set; }
        public int Stamina { get; set; }
        public int RadiationLevel { get; set; }
    }
}