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
        static Player p1, p2, pD, pA;

        [SetUp]
        public void SetUpPlayers()
        {
            p1 = new Player("First", null);
            p2 = new Player("Second", null);
            pD = new Player("Defender", null, 1, 2);
            pA = new Player("Attacker", null, 2, 1);
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

            Assert.That(p1.HasNoArmy());
            Assert.That(p2.Army[UnitType.Ranged] == 1);
        }

        [Test]
        public void TestCavCounter()
        {
            p1.AddUnits(UnitType.Cavalry, 1);
            p2.AddUnits(UnitType.Infantry, 4);

            Combat.ResolveBattle(p1, p2);
            Assert.That(p1.HasNoArmy());
            Assert.That(p2.Army[UnitType.Infantry] == 1);
        }

        [Test]
        public void TestDefenceUsage()
        {
            pD.AddUnits(UnitType.Militia, 100);
            p2.AddUnits(UnitType.Militia, 100);

            Combat.ResolveBattle(p2, pD);
            Assert.That(p2.HasNoArmy());
            Assert.That(pD.Army[UnitType.Militia] == 3);

            pD.AddUnits(UnitType.Militia, 200 - 3);
            p2.AddUnits(UnitType.Militia, 200);

            Combat.ResolveBattle(p2, pD);
            Assert.That(p2.HasNoArmy());
            Assert.That(pD.Army[UnitType.Militia] == 5);
        }

        [Test]
        public void TestAttackUsage()
        {
            pA.AddUnits(UnitType.Militia, 100);
            p2.AddUnits(UnitType.Militia, 105);

            Combat.ResolveBattle(p2, pA);
            Assert.That(p2.HasNoArmy() && pA.HasNoArmy());
        }
    }
}
