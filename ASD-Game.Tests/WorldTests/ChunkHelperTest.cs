﻿using Moq;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using ASD_project.World.Helpers;
using ASD_project.World.Models;
using ASD_project.World.Models.Interfaces;
using ASD_project.World.Models.TerrainTiles;

namespace ASD_Game.Tests.WorldTests
{
    [ExcludeFromCodeCoverage] 
    [TestFixture]
    public class ChunkHelperTest
    {
        private ChunkHelper _sut;

        private Chunk _chunk;
        private ITile[] _tiles;
        private Mock<ChunkHelper> _chunkHelperMock;

        [SetUp]
        public void Setup()
        {
            _tiles = new ITile[] { new GrassTile(1, 1), new GrassTile(1, 2), new GrassTile(1, 3), new GrassTile(1, 4) };
            _chunk = new Chunk(0, 0, _tiles, 6, 2);
            _chunkHelperMock = new Mock<ChunkHelper>();
            _sut = _chunkHelperMock.Object;
            _sut.chunk = _chunk;

        }

        
        public void Test_GetPositionInTileArrayByWorldCoordinates()
        {
            int x = 1;
            int y = 2;

            //Arrange ---------

            //Act ---------

            //Assert ---------
        }



    }
}
