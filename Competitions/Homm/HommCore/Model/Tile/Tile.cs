using System;
using System.Collections.Generic;
using System.Linq;

namespace HoMM
{
    public class Tile
    {
        private List<TileObject> objects;

        public IEnumerable<TileObject> Objects => objects;
        public readonly TileTerrain Terrain;
        public readonly Location Location;

        public bool IsAvailable => Objects.All(x => x.IsPassable) && !isCombat;

        private bool isCombat;

        public Tile(Location location, TileTerrain t, List<TileObject> objects)
        {
            Location = location;
            Terrain = t;

            foreach (var obj in objects)
                obj.Remove += o => objects.Remove(o);

            this.objects = objects;
        }

        public void AddObject(TileObject obj)
        {
            objects.Add(obj);
            obj.Remove += o => objects.Remove(o);
        }

        public Tile(int x, int y, TileTerrain t, List<TileObject> obj) : this(new Location(y, x), t, obj)
        {
        }

        public void BeginCombat()
        {
            if (isCombat)
                throw new InvalidOperationException("Cannot begin combat: another combat has already began");

            isCombat = true;
        }

        public void EndCombat()
        {
            isCombat = false;
        }
    }
}
