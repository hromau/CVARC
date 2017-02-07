using CVARC.V2;
using Infrastructure;
using System;
using System.Collections.Generic;

namespace HoMM
{
    public class NeutralArmy : TileObject, ICombatable
    {
        public int Attack { get; } = 0;
        public int Defence { get; } = 0;
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
            Combat.ResolveBattle(p, this);
            if (this.HasNoArmy())
                OnRemove();
        }

        public static NeutralArmy BuildRandom(Location location, int armyStrength,
            Random random, UnitType preferredUnitType, double preferredUnitShare = 0.5)
        {
            if (preferredUnitShare < 0 || preferredUnitShare > 1)
                throw new ArgumentException("Invalid unit share!");

            int randomizedArmyStrength = (int)(armyStrength * (1 + (random.NextDouble() - 0.5) * 0.4)); //80-120% of original

            var army = new Dictionary<UnitType, int> { };
            foreach (UnitType unit in Enum.GetValues(typeof(UnitType)))
                army.Add(unit, 0);
            int currentStrength = 0;

            int prefStackSize = (int)Math.Floor((randomizedArmyStrength * preferredUnitShare) / UnitConstants.CombatPower[preferredUnitType]);
            army[preferredUnitType] += prefStackSize;
            currentStrength += prefStackSize * UnitConstants.CombatPower[preferredUnitType];

            while(currentStrength <= randomizedArmyStrength)
            {
                var nextUnit = random.Choice<UnitType>();
                army[nextUnit]++;
                currentStrength += UnitConstants.CombatPower[nextUnit];
            }

            return new NeutralArmy(army, location);
        }

        public NeutralArmy Copy(Location location)
        {
            return new NeutralArmy(new Dictionary<UnitType, int>(Army), location);
        }
    }
}
