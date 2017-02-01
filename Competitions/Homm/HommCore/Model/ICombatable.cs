using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM
{
    public interface ICombatable
    {
        int Attack { get; }
        int Defence { get; }
        Dictionary<UnitType, int> Army { get; }


    }

    public static class ICombatableExtensions
    {
        public static bool HasNoArmy(this ICombatable c)
        {
            foreach (var stack in c.Army)
                if (stack.Value > 0)
                    return false;
            return true;
        }
    }
}
