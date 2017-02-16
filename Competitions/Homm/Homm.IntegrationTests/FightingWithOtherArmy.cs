using System.Linq;
using FluentAssertions;
using HoMM;
using NUnit.Framework;

namespace Homm.IntegrationTests
{
    partial class Hero_should
    {
        [Test]
        public void HeroDie_WhenArmyTooWeek()
        {
            MoveHero(Direction.RightDown);
            sensorData.IsDead.Should().BeTrue();
        }

        [Test]
        public void HeroRespawn_WhenLoseBattle()
        {
            MoveHero(Direction.RightDown);
            MoveHero(Direction.Up);
            sensorData.Location.ToLocation().Should().Be(Location.Zero);
        }

        [Test]
        public void LoseUnits_WhenFightWithOtherArmy()
        {
            CollectArmy(1);
            MoveHero(Direction.Up, 5);
            sensorData.MyArmy.Values.Sum().Should().Be(0);
        }

        [Test]
        public void NeutralArmyDies_WhengHeroArmyStronger()
        {
            CollectArmy(10);
            MoveHero(Direction.Up, 5);
            var neutralArmy = GetObjectDataAt(sensorData.Location.X, sensorData.Location.Y).NeutralArmy;
            neutralArmy.Should().BeNull();
        }
    }
}
