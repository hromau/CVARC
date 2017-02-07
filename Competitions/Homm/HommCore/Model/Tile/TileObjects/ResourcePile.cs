using CVARC.V2;
using System;

namespace HoMM
{
    public class ResourcePile : TileObject
    {
        public readonly Resource Resource;
        public readonly int Quantity;

        public override bool IsPassable => true;

        public ResourcePile(Resource resource, int quantity, Location location) : base(location)
        {
            if (quantity <= 0)
                throw new ArgumentException("Cannot create zero or less resources!");
            this.Quantity = quantity;
            this.Resource = resource;
        }

        public override void InteractWithPlayer(Player p)
        {
            p.GainResources(Resource, Quantity);
            OnRemove();
        }
    }
}
