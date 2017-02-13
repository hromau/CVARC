using CVARC.V2;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HoMM
{
    public class ArmiesPair : Combat.CombatResult
    {
        public ArmiesPair(Dictionary<UnitType, int> attacking, Dictionary<UnitType, int> defending) : base(attacking, defending) { }

        internal bool BothAreNotEmpty()
        {
            return AttackingArmy.Any(x => x.Value > 0) && DefendingArmy.Any(x => x.Value > 0);
        }

        internal void Log()
        {
            Debugger.Log("Attackig army:");

            foreach (var kv in AttackingArmy)
                Debugger.Log($"{kv.Key} - {kv.Value}");

            Debugger.Log("Defencing army:");

            foreach (var kv in DefendingArmy)
                Debugger.Log($"{kv.Key} - {kv.Value}");
        }
    }

    public static class Combat
    {
        public class CombatResult
        {
            public Dictionary<UnitType, int> AttackingArmy { get; }
            public Dictionary<UnitType, int> DefendingArmy { get; }

            protected CombatResult(Dictionary<UnitType, int> attacking, Dictionary<UnitType, int> defending)
            {
                AttackingArmy = attacking;
                DefendingArmy = defending;
            }
        }

        internal static void ResolveBattle(ICombatable attacking, ICombatable defending)
        {
            var combatResult = ResolveBattle(new ArmiesPair(attacking.Army, defending.Army));

            attacking.SetArmy(combatResult.AttackingArmy);
            defending.SetArmy(combatResult.DefendingArmy);
        }


        public static CombatResult ResolveBattle(ArmiesPair armies)
        {
            armies.Log();

            while (armies.BothAreNotEmpty())
            {
                var defencingArmyAfterAttack = ResolveOneTurn(armies.AttackingArmy, armies.DefendingArmy);
                var attackingArmyAfterAttack = ResolveOneTurn(armies.DefendingArmy, armies.AttackingArmy);

                armies = new ArmiesPair(attackingArmyAfterAttack, defencingArmyAfterAttack);
                armies.Log();
            }

            return armies;
        }

        private static Dictionary<UnitType, int> ResolveOneTurn(Dictionary<UnitType, int> attackingArmy, Dictionary<UnitType, int> defendingArmy)
        {
            foreach (var attackingUnit in attackingArmy.Where(u => u.Value > 0))
            {
                var attackerType = attackingUnit.Key;
                var attackerCount = attackingUnit.Value;

                var targetOfAttack = defendingArmy
                    .Select(kv => new
                    {
                        UnitType = kv.Key, TotalCount = kv.Value, KilledInCombatCount = GetKilledUnitsCountForTarget(attackerType, attackerCount, kv.Key, kv.Value)
                    })
                    .Select(x => new
                    {
                        x.UnitType, x.TotalCount, x.KilledInCombatCount, Loss = CalculateArmyLossWhenTargetDies(x.UnitType, x.KilledInCombatCount)
                    })
                    .OrderByDescending(x => x.Loss)
                    .ThenByDescending(x => x.KilledInCombatCount)
                    .FirstOrDefault();

                if (targetOfAttack != null)
                    defendingArmy = defendingArmy
                        .ToDictionary(x => x.Key, x => x.Key == targetOfAttack.UnitType ? x.Value - targetOfAttack.KilledInCombatCount : x.Value);
            }

            return defendingArmy.Where(x => x.Value > 0).ToDictionary(x => x.Key, x => x.Value);
        }

        private static int CalculateArmyLossWhenTargetDies(UnitType targetType, int targetCount)
        {
            return UnitConstants.CombatPower[targetType] * targetCount;
        }

        private static int GetKilledUnitsCountForTarget(UnitType attackerType, int attackerCount, UnitType targetType, int targetCount)
        {
            var producedDamage = UnitConstants.CombatPower[attackerType] * attackerCount * UnitConstants.CombatMod[attackerType][targetType];
            var killedUnits = (int)Math.Floor(producedDamage / UnitConstants.CombatPower[targetType]);
            return Math.Min(killedUnits, targetCount);
        }
    }
}
