using System.Linq;
using HoMM;
using NUnit.Framework;
using FluentAssertions;
using HoMM.ClientClasses;

namespace Homm.IntegrationTests
{
    partial class Tests
    {
        [Test]
        public void DontGetResources_WhenLocationIsEmpty()
        {
            MoveHero(Direction.Down);
            sensorData.MyTreasury.Values.Sum().Should().Be(0);
        }

        [Test]
        public void GainThousandResourcesEachType()
        {
            sensorData = client.Move(Direction.Down);
            
            for (var i = 0; i < 4; i++)
            {
                var detectedPile = GetObjectDataAt(0, 2 + i).ResourcePile;
                detectedPile.Resource.ShouldBeEquivalentTo(i);
                sensorData = client.Move(Direction.Down);
                sensorData.MyTreasury.Values.Where(value => value == PilePrice).Should().HaveCount(i+1);
            }
        }

        private MapObjectData GetObjectDataAt(int x, int y)
        {
            return sensorData.Map.Objects.FirstOrDefault(obj => obj.Location.X == x && obj.Location.Y == y);
        }
    }
}
