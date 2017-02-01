using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using HoMM;

namespace HexModelTesting
{
    [TestFixture]
    public class RoundTests
    {
        Round round;

        [SetUp]
        public void PrepareGoodMap()
        {
            round = new Round("TestMaps\\goodMap.txt", new string[] { "First", "Second" });
            round.UpdateTick(new Location[] { new Location(0, 0), new Location(1, 2) });
        }

        [Test]
        public void TestMineCapturing()
        {
            round.UpdateTick(new Location[] { new Location(1, 0), new Location(1, 2) });
            var mine = (Mine)round.Map[new Location(1, 0)].tileObject;
            Assert.That(mine.Owner == round.Players[0]);
            Assert.That(mine.Resource == Resource.Rubles);
            round.DailyTick();
            Assert.AreEqual(round.Players[0].CheckResourceAmount(Resource.Rubles), 1000);
        }

        [Test]
        public void TestResGathering()
        {
            round.UpdateTick(new Location[] { new Location(0, 0), new Location(1, 1) });
            Assert.That(round.Players[1].CheckResourceAmount(Resource.Rubles) == 100);
            Assert.That(round.Map[new Location(1, 1)].tileObject == null);
        }

        [Test]
        public void TestObjectRecapture()
        {
            round.UpdateTick(new Location[] { new Location(0, 0), new Location(2, 1) });
            var obj = (CapturableObject)round.Map[new Location(2, 1)].tileObject;
            Assert.That(obj.Owner == round.Players[1]);
            round.UpdateTick(new Location[] { new Location(2, 1), new Location(0, 0) });
            Assert.That(obj.Owner == round.Players[0]);
        }

        #region player.TryBuyUnits testing
        [Test]
        public void PurchaseFailsWhenNotAtDwelling()
        {
            Assert.False(round.Players[0].TryBuyUnits(1));
            Assert.That(round.Players[0].Army[UnitType.Ranged] == 0);
        }

        [Test]
        public void PurchaseThrowsWhenAskedForNegativeUnits()
        {
            Assert.Throws<ArgumentException>(() => round.Players[0].TryBuyUnits(-1));
        }

        [Test]
        public void PurchaseFailsWhenNoAvailableUnits()
        {
            var p = round.Players[0];
            p.GainResources(Resource.Rubles, 50);
            p.GainResources(Resource.Wood, 1);
            Assert.False(p.TryBuyUnits(1));
            Assert.That(round.Players[0].Army[UnitType.Ranged] == 0);
        }

        [Test]
        public void PurchaseFailsWhenNotEnoughResources()
        {
            for (int i = 0; i < 7; i++)
                round.DailyTick();
            var dwelling = (Dwelling)round.Map[new Location(2, 1)].tileObject;
            Assert.That(dwelling.AvailableUnits == 16);
            Assert.False(round.Players[0].TryBuyUnits(1));
        }

        [Test]
        public void PurchaseSuccessTest()
        {
            for (int i = 0; i < 7; i++)
                round.DailyTick();
            var dwelling = (Dwelling)round.Map[new Location(2, 1)].tileObject;
            round.UpdateTick(new Location[] { new Location(2, 1), new Location(0, 0) });
            var player = round.Players[0];
            player.GainResources(Resource.Rubles, 1000);
            player.GainResources(Resource.Wood, 5);
            player.TryBuyUnits(5);
            Assert.That(player.Army[UnitType.Ranged] == 5);
        }
        #endregion
    }
}
