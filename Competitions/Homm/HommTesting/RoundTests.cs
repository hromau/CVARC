using System;
using System.Collections.Generic;

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
            var mine = (Mine)round.Map[new Location(1, 0)].Objects;
            Assert.That(mine.Owner == round.Players[0]);
            Assert.That(mine.Resource == Resource.Gold);
            round.DailyTick();
            Assert.AreEqual(round.Players[0].CheckResourceAmount(Resource.Gold), 1000);
        }

        [Test]
        public void TestResGathering()
        {
            round.UpdateTick(new Location[] { new Location(0, 0), new Location(1, 1) });
            Assert.That(round.Players[1].CheckResourceAmount(Resource.Gold) == 100);
            Assert.That(round.Map[new Location(1, 1)].Objects == null);
        }

        [Test]
        public void TestObjectRecapture()
        {
            round.UpdateTick(new Location[] { new Location(0, 0), new Location(2, 1) });
            var obj = (CapturableObject)round.Map[new Location(2, 1)].Objects;
            Assert.That(obj.Owner == round.Players[1]);
            round.UpdateTick(new Location[] { new Location(2, 1), new Location(0, 0) });
            Assert.That(obj.Owner == round.Players[0]);
        }

        [Test]
        public void TestGarrisonCaptureOnWin()
        {
            var p = round.Players[0];
            p.AddUnits(UnitType.Infantry, 20);
            p.AddUnits(UnitType.Ranged, 5);
            round.UpdateTick(new Location[] { new Location(3, 3), new Location(0, 0) });
            var garrison = (Garrison)round.Map[new Location(3, 3)].Objects;
            Assert.That(garrison.Owner == p);
        }

        [Test]
        public void TestNeutralArmyRemovalOnWin()
        {
            var p = round.Players[0];
            p.AddUnits(UnitType.Militia, 22);
            p.AddUnits(UnitType.Ranged, 12);
            p.AddUnits(UnitType.Infantry, 7);
            round.UpdateTick(new Location[] { new Location(5, 2), new Location(0, 0) });
            Assert.Null(round.Map[new Location(5, 2)].Objects);
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
            p.GainResources(Resource.Gold, 50);
            p.GainResources(Resource.Wood, 1);
            Assert.False(p.TryBuyUnits(1));
            Assert.That(round.Players[0].Army[UnitType.Ranged] == 0);
        }

        [Test]
        public void PurchaseFailsWhenNotEnoughResources()
        {
            for (int i = 0; i < 7; i++)
                round.DailyTick();
            var dwelling = (Dwelling)round.Map[new Location(2, 1)].Objects;
            Assert.That(dwelling.AvailableUnits == 16);
            Assert.False(round.Players[0].TryBuyUnits(1));
        }

        [Test]
        public void PurchaseSuccessTest()
        {
            for (int i = 0; i < 7; i++)
                round.DailyTick();
            var dwelling = (Dwelling)round.Map[new Location(2, 1)].Objects;
            round.UpdateTick(new Location[] { new Location(2, 1), new Location(0, 0) });
            var player = round.Players[0];
            player.GainResources(Resource.Gold, 1000);
            player.GainResources(Resource.Wood, 5);
            player.TryBuyUnits(5);
            Assert.That(player.Army[UnitType.Ranged] == 5);
        }
        #endregion

        #region player.ExchangeUnitsWithGarrison testing
        [Test]
        public void ExhchangeFailsWhenNotAtGarrison()
        {
            var p = round.Players[0];
            p.AddUnits(UnitType.Infantry, 20);
            p.AddUnits(UnitType.Ranged, 5);
            Assert.False(p.TryExchangeUnitsWithGarrison(p.Army));
        }

        [Test]
        public void ExchangeFailsWhenNotOwnerOfGarrison()
        {
            round.UpdateTick(new Location[] { new Location(3, 3), new Location(0, 0) });
            var p = round.Players[0];
            p.AddUnits(UnitType.Infantry, 20);
            p.AddUnits(UnitType.Ranged, 5);
            var garrison = (Garrison)round.Map[new Location(3, 3)].Objects;
            Assert.That(garrison.Owner != p);
            Assert.False(p.TryExchangeUnitsWithGarrison(p.Army));
        }

        [Test]
        public void ExchangeFailsWhenGivingMoreThatYouHave()
        {
            var p = round.Players[0];
            p.AddUnits(UnitType.Infantry, 20);
            p.AddUnits(UnitType.Ranged, 5);
            var garrison = (Garrison)round.Map[new Location(3, 3)].Objects;
            round.UpdateTick(new Location[] { new Location(3, 3), new Location(0, 0) });
            Assert.That(garrison.Owner == p);
            var unitsToGive = new Dictionary<UnitType, int> { [UnitType.Infantry] = p.Army[UnitType.Infantry] + 1 };
            Assert.False(p.TryExchangeUnitsWithGarrison(unitsToGive));
        }

        [Test]
        public void TestExchangeGivingSuccess()
        {
            var p = round.Players[0];
            p.AddUnits(UnitType.Infantry, 20);
            p.AddUnits(UnitType.Ranged, 5);
            var garrison = (Garrison)round.Map[new Location(3, 3)].Objects;
            round.UpdateTick(new Location[] { new Location(3, 3), new Location(0, 0) });
            Assert.That(garrison.Owner == p && p.Army[UnitType.Infantry] == 10);
            var unitsToGive = new Dictionary<UnitType, int> { [UnitType.Infantry] = 1 };
            Assert.True(p.TryExchangeUnitsWithGarrison(unitsToGive));
            Assert.True(p.Army[UnitType.Infantry] == 10 - 1);
            Assert.True(garrison.Army[UnitType.Infantry] == 1);
        }

        [Test]
        public void ExchangeFailsWhenTakingMoreThanThereIs()
        {
            TestExchangeGivingSuccess();
            var p = round.Players[0];
            var unitsToGive = new Dictionary<UnitType, int> { [UnitType.Infantry] = -2 };
            Assert.False(p.TryExchangeUnitsWithGarrison(unitsToGive));
        }

        [Test]
        public void TestExchangeTakingSuccess()
        {
            TestExchangeGivingSuccess();
            var p = round.Players[0];
            var unitsToGive = new Dictionary<UnitType, int> { [UnitType.Infantry] = -1 };
            Assert.True(p.TryExchangeUnitsWithGarrison(unitsToGive));
        }
        #endregion
    }
}
