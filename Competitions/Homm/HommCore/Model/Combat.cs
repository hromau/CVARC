using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM
{
    public static class Combat
    {
        public static void ResolveBattle(ICombatable p1, ICombatable p2)
        {
            double atkDmgMod = (p1.Attack - p2.Defence) * ((p1.Attack - p2.Defence > 0) ? 0.05 : 0.025);
            double defDmgMod = (p2.Attack - p1.Defence) * ((p2.Attack - p1.Defence > 0) ? 0.05 : 0.025);

            while (!p1.HasNoArmy() && !p2.HasNoArmy())
            {
                var p2NewArmy = ResolveTurn(p1, p2, atkDmgMod);
                var p1NewArmy = ResolveTurn(p2, p1, defDmgMod);

                foreach (var unitType in p1NewArmy.Keys)
                    p1.Army[unitType] = p1NewArmy[unitType];
                foreach (var unitType in p2NewArmy.Keys)
                    p2.Army[unitType] = p2NewArmy[unitType];
            }
        }

        private static Dictionary<UnitType, int> ResolveTurn(ICombatable attacker, ICombatable defender, double atkDmgMod)
        {
            var tempArmyDef = new Dictionary<UnitType, int>(defender.Army);
            foreach (var attStack in attacker.Army.Where(u => u.Value > 0))
            {
                var preferredEnemyOrder = UnitConstants.CombatMod[attStack.Key]
                    .OrderByDescending(kvp => kvp.Value)
                    .Select(kvp => kvp.Key)
                    .ToList();
                var targets = preferredEnemyOrder.Where(u => tempArmyDef[u] > 0);
                if (targets.Count() == 0)
                    break;
                var target = targets.First();
                int kills = ResolveAttack(attStack, new KeyValuePair<UnitType, int>(target, tempArmyDef[target]), atkDmgMod);
                tempArmyDef[target] -= kills;
            }
            return tempArmyDef;
        }

        private static int ResolveAttack(KeyValuePair<UnitType, int> attacker, KeyValuePair<UnitType, int> defender, double atkDmgMod)
        {
            double attackerDamage = UnitConstants.CombatPower[attacker.Key] * attacker.Value
                * UnitConstants.CombatMod[attacker.Key][defender.Key] * (1 + atkDmgMod);
            int killedUnits = (int)Math.Floor(attackerDamage / UnitConstants.CombatPower[defender.Key]);
            return Math.Min(killedUnits, defender.Value);
        }
    }
}
