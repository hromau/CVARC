using System;
using System.Collections.Generic;

namespace HoMM.ClientClasses
{
    public class RealisticArmy : Dictionary<UnitType, RoughQuantity>
    {
        private static readonly int[] Bounds = {1, 5, 10, 20, 40, 60, 80};
        private static readonly int OutOfRangeStep = 25;

        public RealisticArmy() { }

        public RealisticArmy(Dictionary<UnitType, int> army)
        { 
            foreach (var unitType in army.Keys)
            {
                var unitsCount = army[unitType];
                Add(unitType, new RoughQuantity(DistortDown(unitsCount), DistortUp(unitsCount)));
            }
        }

        public RealisticArmy(Dictionary<UnitType, RoughQuantity> army)
        {
            foreach (var kv in army)
                Add(kv.Key, kv.Value);
        }

        private int DistortUp(int number)
        {
            if (number == 0) return 1;

            foreach (var bound in Bounds)
                if (bound > number) return bound - 1;

            return number - (number - Bounds[Bounds.Length -1]) % OutOfRangeStep + OutOfRangeStep - 1;
        }

        private int DistortDown(int number)
        {
            if (number == 0) return 0;

            if (number > Bounds[Bounds.Length - 1])
                return number - (number - Bounds[Bounds.Length - 1]) % OutOfRangeStep;

            for (var i = Bounds.Length - 1; i >= 0; --i)
                if (Bounds[i] <= number) return Bounds[i];

            throw new InvalidProgramException("Should never execute this line of code. Never ever.");
        }
    }
}
