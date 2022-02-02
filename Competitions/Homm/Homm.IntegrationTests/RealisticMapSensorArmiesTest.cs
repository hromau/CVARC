using FluentAssertions;
using HoMM;
using HoMM.ClientClasses;
using NUnit.Framework;
using System;
using System.Linq;

namespace Homm.IntegrationTests
{
    [TestFixture]
    public class RealisticMapSensorArmiesTest
    {
        private HommFinalLevelClient client;

        [SetUp]
        public void RunBeforeAnyTest()
        {
            client = new HommFinalLevelClient();
            client.Configurate("127.0.0.1", 18700, Guid.Empty, operationalTimeLimit: 5, debugMap: true);
        }

        [Test]
        public void FuzzyArmies_ShouldCountainRoughQuantitiesInArmyDictionary()
        {
            var data = client.Wait(0.1);

            foreach (var neutralArmy in data.Map.Objects.Where(x => x.NeutralArmy != null).Select(x => x.NeutralArmy))
            {
                neutralArmy.Army.Count.Should().NotBe(0);
                neutralArmy.Army.Values.Should().AllBeOfType<RoughQuantity>();
            }
        }

        // bounds are 1, 5, 10, 20, 40, 60, 80
        // see SimpleMapSensorArmiesTest.ArmiesDescription to know precise armies sizes
        public static object[][] ArmiesDescription = new object[][]
        {
            new object[] { new Location(0, 1), new RealisticArmy {
                { UnitType.Militia, new RoughQuantity(1, 4) },
                { UnitType.Infantry, new RoughQuantity(1, 4) },
                { UnitType.Ranged, new RoughQuantity(1, 4) },
            }},

            new object[] { new Location(0, 2), new RealisticArmy {
                { UnitType.Militia, new RoughQuantity(1, 4) },
                { UnitType.Cavalry, new RoughQuantity(1, 4) },
            }},

            new object[] { new Location(0, 3), new RealisticArmy {
                { UnitType.Cavalry, new RoughQuantity(1, 4) },
                { UnitType.Militia, new RoughQuantity(10, 19) },
                { UnitType.Infantry, new RoughQuantity(1, 4) },
                { UnitType.Ranged, new RoughQuantity(1, 4) },
            }},

            new object[] { new Location(0, 4), new RealisticArmy {
                { UnitType.Militia, new RoughQuantity(10, 19) },
                { UnitType.Ranged, new RoughQuantity(1, 4) },
            }},

            new object[] { new Location(0, 5), new RealisticArmy {
                { UnitType.Cavalry, new RoughQuantity(5, 9) },
                { UnitType.Infantry, new RoughQuantity(1, 4) },
            }},

            new object[] { new Location(0, 6), new RealisticArmy {
                { UnitType.Militia, new RoughQuantity(10, 19) },
                { UnitType.Cavalry, new RoughQuantity(1, 4) },
            }},

            new object[] { new Location(0, 7), new RealisticArmy {
                { UnitType.Cavalry, new RoughQuantity(1, 4) },
                { UnitType.Infantry, new RoughQuantity(10, 19) },
                { UnitType.Ranged, new RoughQuantity(1, 4) },
            }},
        };

        [Test]
        [TestCaseSource("ArmiesDescription")]
        public void FuzzyArmies_CheckArmy(Location location, RealisticArmy expectedArmy)
        {
            client.Move(Direction.Down);
            client.Move(Direction.RightDown);
            client.Move(Direction.RightUp);
            client.Move(Direction.RightDown);
            var data = client.Move(Direction.RightUp);

            var actualArmy = data.Map.Objects.Single(x => x.Location.ToLocation() == location).NeutralArmy;
            Assert.AreEqual(actualArmy.Army,(expectedArmy));
        }
    }
}
