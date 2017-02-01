using System;

namespace HoMM
{
    public class Dwelling : CapturableObject
    {
        public override bool IsPassable => true;

        public Unit Recruit { get; private set; }
        public int AvailableUnits { get; private set; }

        public Dwelling(Unit unit, Location location, int availableUnits = 0) : base(location)
        {
            if (availableUnits < 0)
                throw new ArgumentException("Cannot have negative units at dwelling!");

            Recruit = unit;
            AvailableUnits = availableUnits;
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
            Owner = p;
        }
    }
}
