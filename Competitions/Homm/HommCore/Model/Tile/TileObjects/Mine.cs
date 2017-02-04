namespace HoMM
{
    public class Mine : CapturableObject
    {
        public Resource Resource { get; private set; }
        
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

        public Mine(Resource res, Location location) : base(location)
        {
            Resource = res;
        }

        public override void InteractWithPlayer(Player p)
        {
            Owner = p;
        }
    }
}
