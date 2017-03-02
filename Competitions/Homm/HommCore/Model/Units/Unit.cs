using HoMM.ClientClasses;
using System;
using System.Collections.Generic;

namespace HoMM
{
    public class Unit
    {
        public string UnitName { get; }
        public UnitType UnitType { get; }
        public int CombatPower => HommRules.Current.Units.CombatPower[UnitType];
        public int WeeklyGrowth => HommRules.Current.Units.WeeklyGrowth[UnitType];
        public Dictionary<Resource, int> UnitCost => HommRules.Current.Units.UnitCost[UnitType];
        public Dictionary<UnitType, double> CombatModAgainst => HommRules.Current.Units.CombatMod[UnitType];

        public Unit(string unitName, UnitType unitType)
        {
            UnitName = unitName;
            UnitType = unitType;
        }
    }
}
