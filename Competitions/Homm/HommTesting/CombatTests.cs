using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using HoMM;
using NUnit.Framework;
using HoMM.ClientClasses;

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
            var result = Combat.Resolve(new ArmiesPair(attacking: twoInfantry, defending: oneInfantry));

            result.AttackingArmy.ShouldAllBeEquivalentTo(new Dictionary<UnitType, int> { { UnitType.Infantry, 1 } });
            result.DefendingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_OneUnit_ShouldLose_TwoUnitsOfSameType()
        {
            var result = Combat.Resolve(new ArmiesPair(attacking: oneInfantry, defending: twoInfantry));

            result.AttackingArmy.Should().BeEmpty();
            result.DefendingArmy.ShouldAllBeEquivalentTo(new Dictionary<UnitType, int> { { UnitType.Infantry, 1 } });
        }

        [Test]
        public void ResolveCombat_TenRanged_ShouldWin_TenInfantry()
        {
            var result = Combat.Resolve(new ArmiesPair(attacking: tenRanged, defending: tenInfantry));
            var countLeft = 10 - (int)Math.Floor(10 * UnitsConstants.Current.CombatMod[UnitType.Infantry][UnitType.Ranged]);

            result.AttackingArmy.ShouldAllBeEquivalentTo(new Dictionary<UnitType, int> { { UnitType.Ranged, countLeft } });
            result.DefendingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_TenInfantry_ShouldWin_FiveCavalry()
        {
            var result = Combat.Resolve(new ArmiesPair(attacking: tenInfantry, defending: fiveCavalry));
            var countLeft = 10 - (int)Math.Floor(5 * UnitsConstants.Current.CombatMod[UnitType.Cavalry][UnitType.Infantry]
                * UnitsConstants.Current.CombatPower[UnitType.Cavalry] / UnitsConstants.Current.CombatPower[UnitType.Infantry]);

            result.AttackingArmy.ShouldAllBeEquivalentTo(new Dictionary<UnitType, int> { { UnitType.Infantry, countLeft } });
            result.DefendingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_FiveCavalry_ShouldWin_TenRanged()
        {
            var result = Combat.Resolve(new ArmiesPair(attacking: fiveCavalry, defending: tenRanged));

            var countLeft = 5 - (int)Math.Floor(10 * UnitsConstants.Current.CombatMod[UnitType.Ranged][UnitType.Cavalry]
                * UnitsConstants.Current.CombatPower[UnitType.Ranged] / UnitsConstants.Current.CombatPower[UnitType.Cavalry]);

            result.AttackingArmy.ShouldAllBeEquivalentTo(new Dictionary<UnitType, int> { { UnitType.Cavalry, countLeft } });
            result.DefendingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_SmallArmy_ShouldLose_MiddleArmy()
        {
            var result = Combat.Resolve(new ArmiesPair(attacking: genericSmallArmy, defending: genericMiddleArmy));

            result.AttackingArmy.Should().BeEmpty();
            result.DefendingArmy.Should().NotBeEmpty();
        }

        [Test]
        public void ResolveCombat_SmallArmy_ShouldLose_BigArmy()
        {
            var result = Combat.Resolve(new ArmiesPair(attacking: genericSmallArmy, defending: genericBigArmy));

            result.AttackingArmy.Should().BeEmpty();
            result.DefendingArmy.Should().NotBeEmpty();
        }

        [Test]
        public void ResolveCombat_MiddleArmy_ShouldLose_BigArmy()
        {
            var result = Combat.Resolve(new ArmiesPair(attacking: genericMiddleArmy, defending: genericBigArmy));

            result.AttackingArmy.Should().BeEmpty();
            result.DefendingArmy.Should().NotBeEmpty();
        }

        [Test]
        public void ResolveCombat_MiddleArmy_ShouldWin_SmallArmy()
        {
            var result = Combat.Resolve(new ArmiesPair(attacking: genericMiddleArmy, defending: genericSmallArmy));

            result.AttackingArmy.Should().NotBeEmpty();
            result.DefendingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_BigArmy_ShouldWin_SmallArmy()
        {
            var result = Combat.Resolve(new ArmiesPair(attacking: genericBigArmy, defending: genericSmallArmy));

            result.AttackingArmy.Should().NotBeEmpty();
            result.DefendingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_BigArmy_ShouldWin_MiddleArmy()
        {
            var result = Combat.Resolve(new ArmiesPair(attacking: genericBigArmy, defending: genericMiddleArmy));

            result.AttackingArmy.Should().NotBeEmpty();
            result.DefendingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_ZergRush_ShouldLose_BalancedArmy()
        {
            var result = Combat.Resolve(new ArmiesPair(attacking: zergRush, defending: balancedArmy));

            result.AttackingArmy.Should().BeEmpty();
            result.DefendingArmy.Should().NotBeEmpty();
        }

        [Test]
        public void ResolveCombat_ZergRush_ShouldWin_RedHerring()
        {
            var result = Combat.Resolve(new ArmiesPair(attacking: zergRush, defending: redHerring));

            result.AttackingArmy.Should().NotBeEmpty();
            result.DefendingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_Should_ProperlyHandleEmptyArmies()
        {
            var firstEmpty = Combat.Resolve(new ArmiesPair(new Dictionary<UnitType, int>(), oneInfantry));

            firstEmpty.AttackingArmy.Should().BeEmpty();
            firstEmpty.DefendingArmy.ShouldAllBeEquivalentTo(new Dictionary<UnitType, int> { {UnitType.Infantry, 1} });

            var secondEmpty = Combat.Resolve(new ArmiesPair(oneInfantry, new Dictionary<UnitType, int>()));

            secondEmpty.AttackingArmy.ShouldAllBeEquivalentTo(new Dictionary<UnitType, int> { {UnitType.Infantry, 1} });
            secondEmpty.DefendingArmy.Should().BeEmpty();

            var bothEmpty = Combat.Resolve(new ArmiesPair(new Dictionary<UnitType, int>(), new Dictionary<UnitType, int>()));

            bothEmpty.AttackingArmy.Should().BeEmpty();
            bothEmpty.DefendingArmy.Should().BeEmpty();
        }

        [Test]
        public void ResolveCombat_ShouldNot_ModifyArgumentsDuringCall()
        {
            Combat.Resolve(new ArmiesPair(oneInfantry, twoInfantry));

            oneInfantry.ShouldAllBeEquivalentTo(new Dictionary<UnitType, int> { {UnitType.Infantry, 1 } });
            twoInfantry.ShouldAllBeEquivalentTo(new Dictionary<UnitType, int> { {UnitType.Infantry, 2} });
        }
    }
}
