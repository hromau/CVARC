using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HoMM.ClientClasses
{
    public class ArmiesPair
    {
        public Dictionary<UnitType, int> AttackingArmy { get; }
        public Dictionary<UnitType, int> DefendingArmy { get; }

        public ArmiesPair(Dictionary<UnitType, int> attacking, Dictionary<UnitType, int> defending)
        {
            AttackingArmy = attacking;
            DefendingArmy = defending;
        }

        internal bool BothAreNotEmpty()
        {
            return !IsEmpty(AttackingArmy) && !IsEmpty(DefendingArmy);
        }

        internal bool IsEmpty(Dictionary<UnitType, int> army)
        {
            return army.All(x => x.Value <= 0);
        }

        internal void Log(string combatState)
        {
            Debugger.Log(combatState);

            Debugger.Log("Attackig army:");

            foreach (var kv in AttackingArmy)
                Debugger.Log($"{kv.Key} - {kv.Value}");

            Debugger.Log("Defending army:");

            foreach (var kv in DefendingArmy)
                Debugger.Log($"{kv.Key} - {kv.Value}");
        }
    }

    public static class Combat
    {
        public class CombatResult : ArmiesPair
        {
            public CombatResult(Dictionary<UnitType, int> attacking, Dictionary<UnitType, int> defending) : base(attacking, defending) { }

            public bool IsAttackerWin => !IsEmpty(AttackingArmy) && IsEmpty(DefendingArmy);
            public bool IsDefenderWin => !IsEmpty(DefendingArmy) && IsEmpty(AttackingArmy);
        }

        internal static void Resolve(ICombatable attacking, ICombatable defending)
        {
            var combatResult = Resolve(new ArmiesPair(attacking.Army, defending.Army));

            attacking.SetArmy(combatResult.AttackingArmy);
            defending.SetArmy(combatResult.DefendingArmy);
        }


        public static CombatResult Resolve(ArmiesPair armies)
        {
            armies.Log("Combat began");

            while (armies.BothAreNotEmpty())
            {
                var defendingArmyAfterAttack = ResolveOneTurn(armies.AttackingArmy, armies.DefendingArmy);
                var attackingArmyAfterAttack = ResolveOneTurn(armies.DefendingArmy, armies.AttackingArmy);

                armies = new ArmiesPair(attackingArmyAfterAttack, defendingArmyAfterAttack);

                armies.Log("Next turn");
            }

            return new CombatResult(armies.AttackingArmy, armies.DefendingArmy);
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
            return HommRules.Current.Units.CombatPower[targetType] * targetCount;
        }

        private static int GetKilledUnitsCountForTarget(UnitType attackerType, int attackerCount, UnitType targetType, int targetCount)
        {
            var producedDamage = HommRules.Current.Units.CombatPower[attackerType] * attackerCount * HommRules.Current.Units.CombatMod[attackerType][targetType];
            var killedUnits = (int)Math.Floor(producedDamage / HommRules.Current.Units.CombatPower[targetType]);
            return Math.Min(killedUnits, targetCount);
        }
    }
}
