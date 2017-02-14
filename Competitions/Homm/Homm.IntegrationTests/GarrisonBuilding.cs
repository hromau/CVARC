using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HoMM;
using NUnit.Framework;

namespace Homm.IntegrationTests
{
    partial class Tests
    {
        [Test]
        public void LoseUnits_WhenBuildGarrison()
        {
            CollectArmy(10);
            sensorData = client.BuildGarrison(sensorData.MyArmy);
            sensorData.MyArmy.Values.Sum().Should().Be(0);
        }

        [Test]
        public void OneGarrisonAppear_WhenBuildingIsDone()
        {
            CollectArmy(10);
            sensorData = client.BuildGarrison(sensorData.MyArmy);
            var newGarrison = GetObjectDataAt(sensorData.Location.X, sensorData.Location.Y).Garrison;
            newGarrison.Should().NotBeNull();
            newGarrison.Owner.Should().NotBeNull();
        }

        private void CollectArmy(int unitsCount)
        {
            GainAllResources();
            var sequenceDirections = new List<Direction>()
            {
                Direction.Down,
                Direction.RightDown,
                Direction.RightUp,
                Direction.RightDown
            };

            foreach (var direction in sequenceDirections)
            {
                MoveHero(direction);
                sensorData = client.PurchaseUnits(unitsCount);
            }
        }

    }
}
