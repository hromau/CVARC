namespace HoMM
{
    public class Mine : CapturableObject, IBuilding
    {
        public Resource Resource { get; private set; }
        public Location BuildingLocation { get; }
        public Location TriggerLocation { get; }

        public override bool IsPassable => true;

        public int Yield
        {
            get
            {
                switch (Resource)
                {
                    case Resource.Gold: return 1000;
                    case Resource.Wood:
                    case Resource.Ore: return 2;
                    default: return 1;
                }
            }
        }

        public Mine(Resource res, Location location) : this(res, location, location) { }

        public Mine(Resource res, Location triggerLocation, Location buildingLocation) : base(triggerLocation)
        {
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
