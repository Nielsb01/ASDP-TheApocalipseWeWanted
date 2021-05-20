using System.Diagnostics.CodeAnalysis;
using DataTransfer.POCO.World.Interfaces;
using DataTransfer.POCO.World.TerrainTiles;
using NUnit.Framework;

namespace WorldGeneration.Tests
{
    [ExcludeFromCodeCoverage]
    public class DirtTileUnitTest
    {
        private ITile _tile;
        private string _tileSymbol;
        
        [SetUp]
        public void Setup()
        {
            _tile = new DirtTile(1,1);
            _tileSymbol = ".";
        }
        
        [Test]
        public void Test_InstanceOf_DirtTile()
        {
            Assert.That(_tile, Is.InstanceOf<DirtTile>());
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
        public void Test_SetX_EqualsTo_5()
        {
            _tile.XPosition = 5;
            Assert.That(_tile.XPosition, Is.EqualTo(5));
        }
        
        [Test]
        public void Test_SetY_EqualsTo_5()
        {
            _tile.YPosition = 5;
            Assert.That(_tile.YPosition, Is.EqualTo(5));
        }
        
        [Test]
        public void Test_TileSymbol_EqualsTo_DirtTileSymbol()
        {
            Assert.That(_tile.Symbol, Is.EqualTo(_tileSymbol));
        }
        
        [Test]
        public void Test_IsAccessible_EqualsTo_True()
        {
            Assert.That(_tile.IsAccessible, Is.EqualTo(true));
        }
    }
}