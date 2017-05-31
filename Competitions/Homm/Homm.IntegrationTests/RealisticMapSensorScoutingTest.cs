using FluentAssertions;
using HoMM;
using HoMM.ClientClasses;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homm.IntegrationTests
{
    [TestFixture]
    public class RealisticMapSensorScoutingTest
    {
        private HommFinalLevelClient client;

        [SetUp]
        public void RunBeforeAnyTest()
        {
            client = new HommFinalLevelClient();
            client.Configurate("127.0.0.1", 18700, Guid.Empty, operationalTimeLimit: 5, debugMap: true);
        }

        [Test]
        public void Scouting_FarMapCorner_ShouldNotBeVisible_WithoutScouting()
        {
            var data = client.Wait(0.1);

            data.Map.Objects
                .Where(x => x.Location.ToLocation().EuclideanDistance(Location.Zero) > HommRules.Current.HeroViewRadius)
                .Should().BeEmpty();

            data.Map.Objects
                .Should().HaveCount(24);
        }

        [Test]
        public void Scouting_OtherHero_ShouldNotBeVisible_WithoutScouting()
        {
            var data = client.Wait(0.1);

            data.Map.Objects
                .Where(x => x.Hero != null)
                .Should().HaveCount(1);

            data.Map.Objects.Single(x => x.Hero != null).Hero.Name.Should().Be("Left");
        }

        [Test]
        public void Scouting_FarMapCorner_ShouldBeVisible_WhenScoutingTile()
        {
            var data = client.ScoutTile(new LocationInfo(7, 7));

            data.Map.Objects
                .Where(x => x.Location.ToLocation().EuclideanDistance(Location.Zero) > HommRules.Current.HeroViewRadius)
                .Should().HaveCount(6);

            data.Map.Objects
                .Where(x => x.Terrain == Terrain.Desert)
                .Should().HaveCount(1);

            data.Map.Objects
                .Where(x => x.Terrain == Terrain.Marsh)
                .Should().HaveCount(5);
        }

        [Test]
        public void Scouting_OtherHero_ShouldBeVisible_WhenScoutingTile_IfInsideScoutingRadius()
        {
            var data = client.ScoutTile(new LocationInfo(7, 7));

            data.Map.Objects
                .Where(x => x.Location.ToLocation().EuclideanDistance(Location.Zero) > HommRules.Current.HeroViewRadius)
                .Where(x => x.Hero != null)
                .Should().NotBeEmpty();

            data.Map.Objects
                .Where(x => x.Location.ToLocation().EuclideanDistance(Location.Zero) > HommRules.Current.HeroViewRadius)
                .Single(x => x.Hero != null)
                .Hero.Name.Should().Be("Right");
        }

        [Test]
        public void Scouting_OtherHero_ShouldBeVisible_WhenScoutingHero()
        {
            var data = client.ScoutHero();

            data.Map.Objects
                .Where(x => x.Location.ToLocation().EuclideanDistance(Location.Zero) > HommRules.Current.HeroViewRadius)
                .Where(x => x.Hero != null)
                .Should().NotBeEmpty();

            data.Map.Objects
                .Where(x => x.Location.ToLocation().EuclideanDistance(Location.Zero) > HommRules.Current.HeroViewRadius)
                .Single(x => x.Hero != null)
                .Hero.Name.Should().Be("Right");
        }
    }
}
