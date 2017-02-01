using System;
using System.Collections.Generic;

namespace HoMM
{
    public class Unit
    {
        public string UnitName { get; }
        public UnitType UnitType { get; }
        public int CombatPower { get { return UnitConstants.CombatPower[UnitType]; } }
        public int WeeklyGrowth { get { return UnitConstants.WeeklyGrowth[UnitType]; } }
        public Dictionary<Resource, int> UnitCost { get {return UnitConstants.UnitCost[UnitType]; } }
        public Dictionary<UnitType, double> CombatModAgainst { get { return UnitConstants.CombatMod[UnitType]; } }

        public Unit(string unitName, UnitType unitType)
        {
            UnitName = unitName;
            UnitType = unitType;
        }
    }
}
