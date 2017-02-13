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
            c.Army.Clear();

            foreach (var kv in army)
                c.Army[kv.Key] = kv.Value;
        }
    }
}
