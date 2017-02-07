using System;
using System.Collections.Generic;
using System.Linq;

namespace HoMM
{
    public class Tile
    {
        public readonly List<TileObject> Objects;
        public readonly TileTerrain Terrain;
        public readonly Location Location;

        public bool IsAvailable => Objects.All(x => x.IsPassable) && !isCombat;

        private bool isCombat;

        public Tile(Location location, TileTerrain t, List<TileObject> obj)
        {
            Location = location;
            Terrain = t;
            Objects = obj;
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

        public void EndFight()
        {
            isCombat = false;
        }
    }
}
