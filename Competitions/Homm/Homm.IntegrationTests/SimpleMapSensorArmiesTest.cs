using FluentAssertions;
using HoMM;
using HoMM.ClientClasses;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Homm.IntegrationTests
{
    [TestFixture]
    public class SimpleMapSensorArmiesTest
    {
        private HommLevel1Client client;

        [SetUp]
        public void RunBeforeAnyTest()
        {
            client = new HommLevel1Client();
            client.Configurate("127.0.0.1", 18700, Guid.Empty, operationalTimeLimit: 5, debugMap: true);
        }

        [Test]
        public void SimpleArmies_ShouldCountainIntegersInArmyDictionary()
        {
            var data = client.Wait(0.1);

            foreach (var neutralArmy in data.Map.Objects.Where(x => x.NeutralArmy != null).Select(x => x.NeutralArmy))
            {
                neutralArmy.Army.Count.Should().NotBe(0);
                neutralArmy.Army.Values.Should().AllBeOfType<int>();
            }
        }
        
        public static object[][] ArmiesDescription = new object[][]
        {
            new object[] { new Location(0, 1), new SimpleArmy {
                { UnitType.Militia, 1 },
                { UnitType.Infantry, 1 },
                { UnitType.Ranged, 3 },
            }},

            new object[] { new Location(0, 2), new SimpleArmy {
                { UnitType.Militia, 1 },
                { UnitType.Cavalry, 3 },
            }},

            new object[] { new Location(0, 3), new SimpleArmy {
                { UnitType.Cavalry, 1 },
                { UnitType.Militia, 11 },
                { UnitType.Infantry, 1 },
                { UnitType.Ranged, 1 },
            }},

            new object[] { new Location(0, 4), new SimpleArmy {
                { UnitType.Militia, 13 },
                { UnitType.Ranged, 2 },
            }},

            new object[] { new Location(0, 5), new SimpleArmy {
                { UnitType.Cavalry, 7 },
                { UnitType.Infantry, 2 },
            }},

            new object[] { new Location(0, 6), new SimpleArmy {
                { UnitType.Militia, 16 },
                { UnitType.Cavalry, 3 },
            }},

            new object[] { new Location(0, 7), new SimpleArmy {
                { UnitType.Cavalry, 1 },
                { UnitType.Infantry, 16 },
                { UnitType.Ranged, 2 },
            }},
        };

        [Test]
        [TestCaseSource("ArmiesDescription")]
        public void SimpleArmies_CheckArmy(Location location, SimpleArmy expectedArmy)
        {
            var data = client.Wait(0.1);
            var actualArmy = data.Map.Objects.Single(x => x.Location.ToLocation() == location).NeutralArmy;
            Assert.Equals(actualArmy.Army,(expectedArmy));
        }
    }
}
