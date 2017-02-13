using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using HoMM;
using NUnit.Framework;

namespace HexModelTesting
{
    [TestFixture]
    public class CombatTests
    {
        private Dictionary<UnitType, int> 
            oneInfantry, 
            twoInfantry,
            tenInfantry,
            tenRanged,
            fiveCavalry,
            genericSmallArmy,
            genericMiddleArmy,
            genericBigArmy,
            zergRush,
            balancedArmy,
            redHerring;

        [OneTimeSetUp]
        public void InvokeOnceBeforeAllTests()
        {
            oneInfantry = new Dictionary<UnitType, int> { {UnitType.Infantry, 1} };
            twoInfantry = new Dictionary<UnitType, int> { {UnitType.Infantry, 2} };

            tenInfantry = new Dictionary<UnitType, int> { {UnitType.Infantry, 10} };
            tenRanged = new Dictionary<UnitType, int> { {UnitType.Ranged, 10} };
            fiveCavalry = new Dictionary<UnitType, int> { { UnitType.Cavalry, 5 } };

            genericSmallArmy = new Dictionary<UnitType, int>
            {
                {UnitType.Infantry, 2 },
                {UnitType.Militia, 2 },
                {UnitType.Cavalry, 1 },
            };

            genericMiddleArmy = new Dictionary<UnitType, int>
            {
                {UnitType.Infantry, 4 },
                {UnitType.Ranged, 4 },
                {UnitType.Cavalry, 2 }
            };

            genericBigArmy = new Dictionary<UnitType, int>
            {
                {UnitType.Infantry, 8 },
                {UnitType.Ranged, 8 },
                {UnitType.Cavalry, 4 }
            };

            zergRush = new Dictionary<UnitType, int>
            {
                {UnitType.Infantry, 30 }
            };

            balancedArmy = new Dictionary<UnitType, int>
            {
                {UnitType.Infantry, 10 },
                {UnitType.Cavalry, 10 },
                {UnitType.Ranged, 10 }
            };

            redHerring = new Dictionary<UnitType, int>
            {
                {UnitType.Infantry, 20 },
                {UnitType.Cavalry, 1 }
            };
        }

        [Test]
        public void ResolveCombat_TwoUnits_ShouldWin_SingleUnitOfSameType()
        {
            var result = Combat.ResolveBattle(new ArmiesPair(attacking: twoInfantry, defencing: oneInfantry));

            result.AttackingArmy.ShouldAllBeEquivalentTo(new Dictionary<UnitType, int> { { UnitType.Infantry, 2 } });
            result.DefencingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_OneUnit_ShouldLose_TwoUnitsOfSameType()
        {
            var result = Combat.ResolveBattle(new ArmiesPair(attacking: oneInfantry, defencing: twoInfantry));

            result.AttackingArmy.Should().BeEmpty();
            result.DefencingArmy.ShouldAllBeEquivalentTo(new Dictionary<UnitType, int> { { UnitType.Infantry, 1 } });
        }

        [Test]
        public void ResolveCombat_TenRanged_ShouldWin_TenInfantry()
        {
            var result = Combat.ResolveBattle(new ArmiesPair(attacking: tenRanged, defencing: tenInfantry));

            result.AttackingArmy[UnitType.Ranged].Should().BeGreaterThan(0);
            result.AttackingArmy.Should().HaveCount(1);
            result.DefencingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_TenInfantry_ShouldWin_FiveCavalry()
        {
            var result = Combat.ResolveBattle(new ArmiesPair(attacking: tenInfantry, defencing: fiveCavalry));

            result.AttackingArmy[UnitType.Infantry].Should().BeGreaterThan(0);
            result.AttackingArmy.Should().HaveCount(1);
            result.DefencingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_FiveCavalry_ShouldWin_TenRanged()
        {
            var result = Combat.ResolveBattle(new ArmiesPair(attacking: fiveCavalry, defencing: tenRanged));

            result.AttackingArmy[UnitType.Cavalry].Should().BeGreaterThan(0);
            result.AttackingArmy.Should().HaveCount(1);
            result.DefencingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_SmallArmy_ShouldLose_MiddleArmy()
        {
            var result = Combat.ResolveBattle(new ArmiesPair(attacking: genericSmallArmy, defencing: genericMiddleArmy));

            result.AttackingArmy.Should().BeEmpty();
            result.DefencingArmy.Should().NotBeEmpty();
        }

        [Test]
        public void ResolveCombat_SmallArmy_ShouldLose_BigArmy()
        {
            var result = Combat.ResolveBattle(new ArmiesPair(attacking: genericSmallArmy, defencing: genericBigArmy));

            result.AttackingArmy.Should().BeEmpty();
            result.DefencingArmy.Should().NotBeEmpty();
        }

        [Test]
        public void ResolveCombat_MiddleArmy_ShouldLose_BigArmy()
        {
            var result = Combat.ResolveBattle(new ArmiesPair(attacking: genericMiddleArmy, defencing: genericBigArmy));

            result.AttackingArmy.Should().BeEmpty();
            result.DefencingArmy.Should().NotBeEmpty();
        }

        [Test]
        public void ResolveCombat_MiddleArmy_ShouldWin_SmallArmy()
        {
            var result = Combat.ResolveBattle(new ArmiesPair(attacking: genericMiddleArmy, defencing: genericSmallArmy));

            result.AttackingArmy.Should().NotBeEmpty();
            result.DefencingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_BigArmy_ShouldWin_SmallArmy()
        {
            var result = Combat.ResolveBattle(new ArmiesPair(attacking: genericBigArmy, defencing: genericSmallArmy));

            result.AttackingArmy.Should().NotBeEmpty();
            result.DefencingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_BigArmy_ShouldWin_MiddleArmy()
        {
            var result = Combat.ResolveBattle(new ArmiesPair(attacking: genericBigArmy, defencing: genericMiddleArmy));

            result.AttackingArmy.Should().NotBeEmpty();
            result.DefencingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_ZergRush_ShouldLose_BalancedArmy()
        {
            var result = Combat.ResolveBattle(new ArmiesPair(attacking: zergRush, defencing: balancedArmy));

            result.AttackingArmy.Should().BeEmpty();
            result.DefencingArmy.Should().NotBeEmpty();
        }

        [Test]
        public void ResolveCombat_ZergRush_ShouldWin_RedHerring()
        {
            var result = Combat.ResolveBattle(new ArmiesPair(attacking: zergRush, defencing: redHerring));

            result.AttackingArmy.Should().NotBeEmpty();
            result.DefencingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_Should_ProperlyHandleEmptyArmies()
        {
            var firstEmpty = Combat.ResolveBattle(new ArmiesPair(new Dictionary<UnitType, int>(), oneInfantry));

            firstEmpty.AttackingArmy.Should().BeEmpty();
            firstEmpty.DefencingArmy.ShouldAllBeEquivalentTo(new Dictionary<UnitType, int> { {UnitType.Infantry, 1} });

            var secondEmpty = Combat.ResolveBattle(new ArmiesPair(oneInfantry, new Dictionary<UnitType, int>()));

            secondEmpty.AttackingArmy.ShouldAllBeEquivalentTo(new Dictionary<UnitType, int> { {UnitType.Infantry, 1} });
            secondEmpty.DefencingArmy.Should().BeEmpty();

            var bothEmpty = Combat.ResolveBattle(new ArmiesPair(new Dictionary<UnitType, int>(), new Dictionary<UnitType, int>()));

            bothEmpty.AttackingArmy.Should().BeEmpty();
            bothEmpty.DefencingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_ShouldNot_ModifyArgumentsDuringCall()
        {
            Combat.ResolveBattle(new ArmiesPair(oneInfantry, twoInfantry));

            oneInfantry.ShouldAllBeEquivalentTo(new Dictionary<UnitType, int> { {UnitType.Infantry, 1 } });
            twoInfantry.ShouldAllBeEquivalentTo(new Dictionary<UnitType, int> { {UnitType.Infantry, 2} });
        }
    }
}
