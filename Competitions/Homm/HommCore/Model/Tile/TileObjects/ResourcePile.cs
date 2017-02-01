using System;

namespace HoMM
{
    public class ResourcePile : TileObject
    {
        public readonly Resource resource;
        public readonly int quantity;

        public override bool IsPassable => true;

        public ResourcePile(Resource resource, int quantity, Location location) : base(location)
        {
            if (quantity <= 0)
                throw new ArgumentException("Cannot create zero or less resources!");
            this.quantity = quantity;
            this.resource = resource;
        }

        public override void InteractWithPlayer(Player p)
        {
            p.GainResources(resource, quantity);
            OnRemove();
        }
    }
}
