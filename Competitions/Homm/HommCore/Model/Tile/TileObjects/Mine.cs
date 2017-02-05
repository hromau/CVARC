using System;

namespace HoMM
{
    public class Mine : CapturableObject, IBuilding
    {
        public Resource Resource { get; private set; }
        public Location BuildingLocation { get; }
        public Location TriggerLocation { get; }

        public override bool IsPassable => true;

        public int Yield => 10;

        public Mine(Resource res, Location location) : this(res, location, location) { }

        public Mine(Resource res, Location triggerLocation, Location buildingLocation) : base(triggerLocation)
        {
            if (triggerLocation == null)
                throw new ArgumentException("Expected triggerLocation, got null");

            if (buildingLocation == null)
                throw new ArgumentException("Expected buildingLocation, got null");

            Resource = res;
            TriggerLocation = triggerLocation;
            BuildingLocation = buildingLocation;
        }

        public override void InteractWithPlayer(Player p)
        {
            Owner = p;
        }
    }
}
