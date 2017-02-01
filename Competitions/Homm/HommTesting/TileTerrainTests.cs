using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using HoMM;

namespace HexModelTesting
{
    [TestFixture]
    public class TileTerrainTests
    {
        [Test]
        public void AddGoodTerrain()
        {
            TileTerrain t = TileTerrain.Marsh;
            Assert.AreEqual(t.TravelCost, 1.75, 0.0001);
        }

        [Test]
        public void EditCorrectlyFixesTravelCost()
        {
            var t = TileTerrain.Grass;
            Assert.AreEqual(t.TravelCost, 1, 0.0001);
            t = TileTerrain.Arid;
            Assert.AreEqual(t.TravelCost, 1.25, 0.0001);
        }
    }
}
