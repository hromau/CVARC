using System.Collections.Generic;

namespace HoMM
{
    public class Tile
    {
        public readonly List<TileObject> Objects;
        public readonly TileTerrain Terrain;
        public readonly Location Location;

        public Tile(Location location, TileTerrain t, List<TileObject> obj)
        {
            Location = location;
            Terrain = t;
            Objects = obj;
        }

        public Tile(int x, int y, TileTerrain t, List<TileObject> obj) : this(new Location(y, x), t, obj)
        {
        }
    }
}
