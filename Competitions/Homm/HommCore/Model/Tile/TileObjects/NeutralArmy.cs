using CVARC.V2;
using HoMM.ClientClasses;
using Infrastructure;
using System;
using System.Collections.Generic;

namespace HoMM
{
    public class NeutralArmy : TileObject, ICombatable
    {
        public Dictionary<UnitType, int> Army { get; private set; }
        public CapturableObject GuardedObject { get; private set; }

        public override bool IsPassable => true;

        public NeutralArmy(Dictionary<UnitType, int> army, Location location) : base(location)
        {
            Army = army;

            foreach (var kv in army)
                Debugger.Log($"Init {kv.Value} {kv.Key} at {location}");
        }

        public void GuardObject(CapturableObject obj)
        {
            GuardedObject = obj;
        }

        public override void InteractWithPlayer(Player p)
        {
            Combat.Resolve(p, this);
            if (this.HasNoArmy())
                OnRemove();
        }

        public static NeutralArmy BuildRandom(Location location, int armyStrength, Random random)
        {
            armyStrength = (int)(armyStrength * (1 + (random.NextDouble() - 0.5) * 0.4));  // 80-120% of original

            var dominatingUnitType = random.Choice<UnitType>();
            var dominationFraction = 0.7;

            var dominatorCombatPower = HommRules.Current.Units.CombatPower[dominatingUnitType];
            var dominatingUnitsCount = (int)Math.Ceiling((armyStrength * dominationFraction) / dominatorCombatPower);

            var army = new Dictionary<UnitType, int> { { dominatingUnitType, dominatingUnitsCount } };
            var currentStrength = dominatingUnitsCount * HommRules.Current.Units.CombatPower[dominatingUnitType];

            while(currentStrength < armyStrength)
            {
                var nextUnit = random.Choice<UnitType>();
                army[nextUnit] = army.GetOrDefault(nextUnit, 0) + 1;
                currentStrength += HommRules.Current.Units.CombatPower[nextUnit];
            }

            return new NeutralArmy(army, location);
        }

        public NeutralArmy Copy(Location location)
        {
            return new NeutralArmy(new Dictionary<UnitType, int>(Army), location);
        }

        public void SetUnitsCount(UnitType unitType, int count)
        {
            Army[unitType] = count;
        }
    }
}
