using System.Collections.Generic;

namespace HoMM
{
    public class Garrison : CapturableObject, ICombatable
    {
        public override bool IsPassable => true;

        public int Attack { get; } = 0;
        public int Defence { get; } = 0;
        public Dictionary<UnitType, int> Army { get; private set; }
        public Garrison(Dictionary<UnitType, int> guards, Location location) : base(location)
        {
            Army = guards;
        }

        public override void InteractWithPlayer(Player p)
        {
            if (p != Owner)
            {
                Combat.ResolveBattle(p, this);
                if (this.HasNoArmy())
                    Owner = p;
            }
        }
    }
}
