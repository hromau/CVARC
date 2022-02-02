using CVARC.V2;
using FluentAssertions;
using HoMM;
using HoMM.ClientClasses;
using NUnit.Framework;
using System;
using System.Linq;

namespace Homm.IntegrationTests
{
    [TestFixture]
    class ScoutingGameplayTest
    {
        private HommFinalLevelClient client;

        [SetUp]
        public void RunBeforeAnyTest()
        {
            client = new HommFinalLevelClient();
            client.Configurate("127.0.0.1", 18700, Guid.Empty, operationalTimeLimit: 5, debugMap: true);
        }

        [Test]
        public void Scouting_ShouldNotSeeScoutedTiles_OnNextTurn()
        {
            client.ScoutTile(new LocationInfo(7, 7));
            var data = client.Wait(0.1);

            data.Map.Objects
                .Where(x => x.Location.ToLocation().EuclideanDistance(Location.Zero) > HommRules.Current.HeroViewRadius)
                .Should().BeEmpty();
        }

        [Test]
        public void Scouting_ShouldNotSeeScoutedHero_OnNextTurn()
        {
            client.ScoutHero();
            var data = client.Wait(0.1);

            data.Map.Objects
                .FirstOrDefault(x => x.Hero != null && x.Hero.Name == "Right")
                .Should().BeNull();
        }

        [Test]
        public void Scouting_ShouldThrowException_IfScoutingHeroOnCooldown()
        {
            client.ScoutHero();

            Action scout = () => client.ScoutHero();
            Assert.Throws<ClientException>(scout.Invoke);
            // scout.ShouldThrow<ClientException>();
        }

        [Test]
        public void Scouting_ShouldThrowException_IfScoutingTileOnCooldown()
        {
            client.ScoutTile(new LocationInfo(0, 0));

            Action scout = () => client.ScoutTile(new LocationInfo(0, 0));
            Assert.Throws<ClientException>(scout.Invoke);
            // scout.ShouldThrow<ClientException>();
        }

        [Test]
        public void Scouting_ShouldThrowException_IfScoutingTileAfterHeroCooldown()
        {
            client.ScoutHero();

            Action scout = () => client.ScoutTile(new LocationInfo(0, 0));
            Assert.Throws<ClientException>(scout.Invoke);
            // scout.ShouldThrow<ClientException>();
        }

        [Test]
        public void Scouting_ShouldThrowException_IfScoutingHeroAfterTileCooldown()
        {
            client.ScoutTile(new LocationInfo(0, 0));

            Action scout = () => client.ScoutHero();
            Assert.Throws<ClientException>(scout.Invoke);
            // scout.ShouldThrow<ClientException>();
        }

        [Test]
        public void Scouting_ShouldThrowException_WhenLocationIsOutOfMapBounds()
        {
            Action scout = () => client.ScoutTile(new LocationInfo(-1, -1));
            Assert.Throws<ClientException>(scout.Invoke);
            // scout.ShouldThrow<ClientException>();
        }

        [Test]
        public void Scouting_ShouldBeAvailable_AfterCooldown()
        {
            client.ScoutHero();
            client.Wait(HommRules.Current.ScoutingCooldown);

            Action scout = () => client.ScoutHero();
            Assert.Throws<ClientException>(scout.Invoke);
            // scout.ShouldNotThrow();
        }
    }
}
