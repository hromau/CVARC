using System;

namespace HoMM
{
    public class Dwelling : CapturableObject, IBuilding
    {
        public override bool IsPassable => true;

        public Location BuildingLocation { get; }
        public Location EntryLocation { get; }

        public Unit Recruit { get; private set; }
        public int AvailableUnits { get; private set; }

        public Dwelling(UnitType unit, Location location, int availableUnits = 0)
            : this(unit, location, location, availableUnits) { }

        public Dwelling(UnitType unit, Location triggerLocation, Location buildingLocation, int availableUnits = 0) 
            : base(triggerLocation)
        {
            if (availableUnits < 0)
                throw new ArgumentException("Cannot have negative units at dwelling!");

            if (triggerLocation == null)
                throw new ArgumentException("Expected triggerLocation, got null");

            if (buildingLocation == null)
                throw new ArgumentException("Expected buildingLocation, got null");

            Recruit = UnitFactory.FromType(unit);
            AvailableUnits = availableUnits;
            EntryLocation = triggerLocation;
            BuildingLocation = buildingLocation;
        }

        
        public void AddWeeklyGrowth()
        {
            AvailableUnits += Recruit.WeeklyGrowth;
        }
        public void RemoveBoughtUnits(int amount)
        {
            AvailableUnits -= amount;
        }

        public override void InteractWithPlayer(Player p)
        {
            if (p == Owner) return;

            Owner = p;
            p.OnDwellingCaptured(this);
        }
    }
}
