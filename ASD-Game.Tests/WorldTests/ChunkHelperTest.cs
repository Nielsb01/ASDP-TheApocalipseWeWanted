﻿using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldGeneration.Helper;
using WorldGeneration.Models;
using WorldGeneration.Models.Interfaces;
using WorldGeneration.Models.TerrainTiles;

namespace WorldGeneration.Tests
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
