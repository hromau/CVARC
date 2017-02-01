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
    }
}
