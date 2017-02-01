namespace HoMM
{
    public class Tile
    {
        public TileObject tileObject;
        public TileTerrain tileTerrain;
        public readonly Location location;

        public Tile(Location location, TileTerrain t, TileObject obj)
        {
            this.location = location;
            tileTerrain = t;
            tileObject = obj;
        }

        public Tile(int x, int y, TileTerrain t, TileObject obj) : this(new Location(y, x), t, obj)
        {
        }
    }
}
