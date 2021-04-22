using NUnit.Framework;
using WorldGeneration.Models.Interfaces;
using WorldGeneration.Models.TerrainTiles;

namespace WorldGeneration.Tests
{
    public class GrassTileUnitTest
    {
        private ITerrainTile _tile;
        
        [SetUp]
        public void Setup()
        {
            _tile = new GrassTile();
        }
        
        [Test]
        public void Test_InstanceOf_GrassTile()
        {
            Assert.That(_tile, Is.InstanceOf<GrassTile>());
        }
        
        [Test]
        public void Test_InstanceOf_TerrainTile()
        {
            Assert.That(_tile, Is.InstanceOf<ITerrainTile>());
        }
        
        [Test]
        public void Test_InstanceOf_Tile()
        {
            Assert.That(_tile, Is.InstanceOf<ITile>());
        }
        
        [Test]
        public void Test_Symbol_EqualsTo_GrassTileSymbol()
        {
            Assert.That(_tile.Symbol, Is.EqualTo(","));
        }
    }
}