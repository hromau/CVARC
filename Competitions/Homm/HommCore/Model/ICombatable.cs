using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM
{
    public interface ICombatable
    {
        Dictionary<UnitType, int> Army { get; }
        string UnityId { get; }

        void SetUnitsCount(UnitType unitType, int count);
    }

    static class CombatableExtensions
    {
        public static bool HasNoArmy(this ICombatable c)
        {
            foreach (var stack in c.Army)
                if (stack.Value > 0)
                    return false;
            return true;
        }

        public static void SetArmy(this ICombatable c, Dictionary<UnitType, int> army)
        {
            if (army == c.Army) return;

            foreach (var kv in c.Army.Where(kv => !army.ContainsKey(kv.Key)).ToArray())
                c.SetUnitsCount(kv.Key, 0);

            foreach (var kv in army)
                c.SetUnitsCount(kv.Key, kv.Value);
        }
    }
}
