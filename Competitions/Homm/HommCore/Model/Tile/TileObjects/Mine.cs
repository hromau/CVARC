using HoMM.ClientClasses;
using System;

namespace HoMM
{
    public class Mine : CapturableObject, IBuilding
    {
        public Resource Resource { get; private set; }
        public Location BuildingLocation { get; }
        public Location EntryLocation { get; }

        public override bool IsPassable => true;

        public int Yield => HommRules.Current.MineDailyResourceYield;

        public Mine(Resource res, Location location) : this(res, location, location) { }

        public Mine(Resource res, Location entryLocation, Location buildingLocation) : base(entryLocation)
        {
            if (entryLocation == null)
                throw new ArgumentException("Expected triggerLocation, got null");

            if (buildingLocation == null)
                throw new ArgumentException("Expected buildingLocation, got null");

            Resource = res;
            EntryLocation = entryLocation;
            BuildingLocation = buildingLocation;
        }

        public override void InteractWithPlayer(Player p)
        {
            if (p == Owner) return;

            Owner = p;
        }
    }
}
