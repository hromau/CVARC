using System.Linq;
using FluentAssertions;
using HoMM;
using NUnit.Framework;
using Dwelling = HoMM.ClientClasses.Dwelling;

namespace Homm.IntegrationTests
{
    partial class Hero_should
    {
        private void GainAllResources()
        {
            MoveHero(Direction.Down, 7);
            MoveHero(Direction.RightDown);
            MoveHero(Direction.Up, 5);
            MoveHero(Direction.RightDown);
        }

        [Test]
        public void DontBuy_WhenCurrentLocationIsEmpty()
        {
            GainAllResources();
            sensorData = client.PurchaseUnits(100);
            sensorData.MyTreasury.Values.All(val => val == MaxResourceCount).Should().BeTrue();
            sensorData.MyArmy.Values.All(val => val == 0).Should().BeTrue();
        }

        [Test]
        public void BuyingInfantry()
        {
          GainAllResources(); 
          MoveHero(Direction.Down);
          CheckUnitsBuying(UnitType.Infantry);
        }

        [Test]
        public void BuyingRanged()
        {
            GainAllResources();
            MoveHero(Direction.Down, Direction.RightDown);
            CheckUnitsBuying(UnitType.Ranged);
        }

        [Test]
        public void BuyingCavalry()
        {
            GainAllResources();
            MoveHero(Direction.RightDown, 2);
            CheckUnitsBuying(UnitType.Cavalry);
        }

        [Test]
        public void BuyingMilita()
        {
            GainAllResources();
            MoveHero(Direction.RightDown, 3);
            CheckUnitsBuying(UnitType.Militia);
        }

        private void CheckUnitsBuying(UnitType requestedUnitType)
        {
            var dwelling = GetObjectDataAt(sensorData.Location.X, sensorData.Location.Y).Dwelling;
            dwelling.UnitType.Should().Be(requestedUnitType);
            var requestedCount = dwelling.AvailableToBuyCount;

            sensorData = client.PurchaseUnits(requestedCount);
            CheckUnitsCount(dwelling, requestedCount);
            CheckTreasuryAfterUnitBuying(dwelling, requestedCount);
        }

        private void CheckUnitsCount(Dwelling dwelling, int requestedCount)
        {
            sensorData.MyArmy[dwelling.UnitType].Should().Be(requestedCount);
            sensorData.MyArmy.Values.Sum().Should().Be(requestedCount);
        }

        private void CheckTreasuryAfterUnitBuying(Dwelling dwelling, int requestedCount)
        {
            var unitCost = UnitConstants.UnitCost[dwelling.UnitType];
            sensorData.MyTreasury.Where(res => !unitCost.ContainsKey(res.Key) && res.Value == MaxResourceCount)
                                 .Should().HaveCount(4 - unitCost.Keys.Count);

            foreach (var resourceType in unitCost.Keys)
            {
                var remainingAmount = MaxResourceCount - requestedCount * unitCost[resourceType];
                sensorData.MyTreasury[resourceType].Should().Be(remainingAmount);
            }
        }
    }
}
