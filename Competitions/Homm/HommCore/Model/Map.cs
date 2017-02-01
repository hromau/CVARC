using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HoMM
{
    public class Map : IEnumerable<Tile>
    {
        Tile[,] map;

        public int Height { get { return map.GetLength(0); } }
        public int Width { get { return map.GetLength(1); } }
        public MapSize Size { get { return new MapSize(Height, Width); } }
        public Tile this[Location location] { get { return map[location.Y, location.X]; } }

        public Map(int width, int height)
        {
            map = new Tile[height, width];
        }

        public Map(string filename)
        {
            var input = File.ReadAllLines(filename);
            var height = input.Length;
            var width = input[0].Split().Length;
            map = new Tile[height, width];

            for (int y = 0; y < height; y++)
            {
                var line = input[y].Split().Where(s => s != "").ToArray();
                for (int x = 0; x < width; x++)
                    map[y, x] = MakeTile(x, y, line[x]);
            }

            AssignGuardsToCapturableObjs();
        }

        private void AssignGuardsToCapturableObjs()
        {
            foreach (var tile in map)
                if (tile.tileObject is NeutralArmy)
                {
                    var neutralArmy = (NeutralArmy)tile.tileObject;
                    var neighb = neutralArmy.location.Neighborhood.Inside(Size);
                    foreach (var t in neighb.Select(pt => map[pt.Y, pt.X]))
                        if (t.tileObject is CapturableObject)
                            neutralArmy.GuardObject((CapturableObject)t.tileObject);
                }
        }

        public Map(int width, int height, IEnumerable<Tile> tiles)
            : this(width, height)
        {
            foreach (var tile in tiles)
                map[tile.location.X, tile.location.Y] = tile;
        }

        public Tile MakeTile(int x, int y, string s)
        {
            TileTerrain t = InitTerrain(char.ToUpper(s[0]));
            TileObject obj = InitObject(s, new Location(y, x));
            var tile = new Tile(x, y, t, obj);
            if (tile.tileObject != null)
                tile.tileObject.Remove += (o) => tile.tileObject = null;
            return tile;
        }

        private TileTerrain InitTerrain(char c)
        {
            return TileTerrain.Parse(c);
        }

        private TileObject InitObject(string s, Location location)
        {
            switch (s[1])
            {
                case '*':
                    {
                        var resName = Enum.GetNames(typeof(Resource))
                            .SingleOrDefault(res => res[0] == s[2]);
                        var resource = (Resource)Enum.Parse(typeof(Resource), resName == null ? "Rubles" : resName);
                        return new Mine(resource, location);
                    }
                case 'p':
                    {
                        var resName = Enum.GetNames(typeof(Resource))
                            .SingleOrDefault(res => res[0] == s[2]);
                        var resource = (Resource)Enum.Parse(typeof(Resource), resName == null ? "Rubles" : resName);
                        int amount = int.Parse(s.Substring(3));
                        return new ResourcePile(resource, amount, location);
                    }
                case 'M':
                    {
                        return CreateNeutralArmyFromString(s, location);
                    }
                case 'D':
                    {
                        var recriutTypeName = Enum.GetNames(typeof(UnitType))
                            .SingleOrDefault(res => res[0] == s[2]);
                        var unitType = (UnitType)Enum.Parse(typeof(UnitType), recriutTypeName);
                        return new Dwelling(UnitFactory.CreateFromUnitType(unitType), location);
                    }
                case '-':
                    return null;
                default:
                    throw new ArgumentException("Unknown object!");
            }
        }

        private NeutralArmy CreateNeutralArmyFromString(string s, Location location)
        {
            var monsterTypeName = Enum.GetNames(typeof(UnitType))
                .SingleOrDefault(res => res[0] == s[2]);
            var unitType = (UnitType)Enum.Parse(typeof(UnitType), monsterTypeName);
            int amount = int.Parse(s.Substring(3).Split('.')[0]);
            return new NeutralArmy(new Dictionary<UnitType, int> { [unitType] = amount }, location);
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            foreach (var tile in map)
                yield return tile;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
