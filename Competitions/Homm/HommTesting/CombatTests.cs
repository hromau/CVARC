using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HoMM;
using NUnit.Framework;

namespace HexModelTesting
{
    [TestFixture]
    public class CombatTests
    {
        static Player p1, p2;

        [SetUp]
        public void SetUpPlayers()
        {
            p1 = new Player("First", null);
            p2 = new Player("Second", null);
        }

        [Test]
        public void Test2v1Combat()
        {
            p1.AddUnits(UnitType.Infantry, 2);
            p2.AddUnits(UnitType.Infantry, 1);
            Combat.ResolveBattle(p1, p2);

            Assert.That(p1.Army[UnitType.Infantry] == 2 - 1);
            Assert.That(p2.Army[UnitType.Infantry] == 1 - 1);
        }

        [Test]
        public void TestUnitCounters()
        {
            p1.AddUnits(UnitType.Infantry, 1);
            p2.AddUnits(UnitType.Ranged, 1);
            Combat.ResolveBattle(p1, p2);

            Assert.That(p1.HasNoArmy);
            Assert.That(p2.Army[UnitType.Ranged] == 1);
        }

        [Test]
        public void TestCavCounter()
        {
            p1.AddUnits(UnitType.Cavalry, 1);
            p2.AddUnits(UnitType.Infantry, 4);

            Combat.ResolveBattle(p1, p2);
            Assert.That(p1.HasNoArmy);
            Assert.That(p2.Army[UnitType.Infantry] == 1);
        }
    }
}
