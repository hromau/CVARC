using System;
using System.Collections.Generic;

namespace HoMM
{
    public static class UnitConstants
    {
        public static Dictionary<UnitType, int> WeeklyGrowth = new Dictionary<UnitType, int>
        {
            [UnitType.Infantry] = 16,
            [UnitType.Ranged] = 16,
            [UnitType.Cavalry] = 8,
            [UnitType.Militia] = 16
        };

        public static Dictionary<UnitType, Dictionary<Resource, int>> UnitCost = new Dictionary<UnitType, Dictionary<Resource, int>>
        {
            [UnitType.Infantry] = new Dictionary<Resource, int> { [Resource.Gold] = 1, [Resource.Iron] = 1 },
            [UnitType.Ranged] = new Dictionary<Resource, int> { [Resource.Gold] = 1, [Resource.Glass] = 1 },
            [UnitType.Cavalry] = new Dictionary<Resource, int> { [Resource.Gold] = 2, [Resource.Ebony] = 2 },
            [UnitType.Militia] = new Dictionary<Resource, int> { [Resource.Gold] = 1 }
        };

        public static Dictionary<UnitType, int> CombatPower = new Dictionary<UnitType, int>
        {
            [UnitType.Infantry] = 15,
            [UnitType.Ranged] = 15,
            [UnitType.Cavalry] = 30,
            [UnitType.Militia] = 12
        };

        public static Dictionary<UnitType, Dictionary<UnitType, double>> CombatMod =
            new Dictionary<UnitType, Dictionary<UnitType, double>>
            {
                [UnitType.Infantry] = new Dictionary<UnitType, double>
                {
                    [UnitType.Infantry] = 1,
                    [UnitType.Ranged] = 0.75,
                    [UnitType.Cavalry] = 1.25,
                    [UnitType.Militia] = 1
                },

                [UnitType.Ranged] = new Dictionary<UnitType, double>
                {
                    [UnitType.Infantry] = 1.25,
                    [UnitType.Ranged] = 1,
                    [UnitType.Cavalry] = 0.75,
                    [UnitType.Militia] = 1
                },

                [UnitType.Cavalry] = new Dictionary<UnitType, double>
                {
                    [UnitType.Infantry] = 0.75,
                    [UnitType.Ranged] = 1.25,
                    [UnitType.Cavalry] = 1,
                    [UnitType.Militia] = 1
                },

                [UnitType.Militia] = new Dictionary<UnitType, double>
                {
                    [UnitType.Infantry] = 1,
                    [UnitType.Ranged] = 1,
                    [UnitType.Cavalry] = 1,
                    [UnitType.Militia] = 1
                }
            };
    }
}
