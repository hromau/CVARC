using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HoMM;
using NUnit.Framework;

namespace HexModelTesting
{
    [TestFixture]
    public class MapTests
    {
        [Test]
        public void Test1TileMap()
        {
            var m = new Map("TestMaps\\map1.txt");
            Assert.AreEqual(m.Height, 1);
            Assert.AreEqual(m.Width, 1);
            Assert.IsEmpty(Location.Zero.Neighborhood.Inside(m.Size));
        }

        [Test]
        public void InitOfBadTerrainThrows()
        {
            Assert.Throws<ArgumentException>(
                () => new Map("TestMaps\\badTerrain.txt"),
                "Unknown terrain type!",
                new object[] { });
        }

        [Test]
        public void InitOfBadObjectThrows()
        {
            Assert.Throws<ArgumentException>(
                () => new Map("TestMaps\\badObject.txt"),
                "Unknown object!",
                new object[] { });
        }

        [Test]
        public void TestGoodMap()
        {
            var m = new Map("TestMaps\\goodMap.txt");
            Assert.AreEqual(m.Height, 6);
            Assert.AreEqual(m.Width, 5);
            var neighbours = new Location(5, 4).Neighborhood.Inside(m.Size).ToList();
            Assert.AreEqual(neighbours.Count, 3);
            var expected = new List<Location> { new Location(4, 3), new Location(5, 3), new Location(4, 4) };
            CollectionAssert.AreEquivalent(neighbours, expected);
        }

        [Test]
        public void TestMapWithUnits()
        {
            var m = new Map("TestMaps\\mapWithUnits.txt");
            //Assert.True(m[])
        }
    }
}
