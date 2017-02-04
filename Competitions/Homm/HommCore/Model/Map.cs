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
                foreach (var tileObject in tile.Objects)
                    if (tileObject is NeutralArmy)
                    {
                        var neutralArmy = (NeutralArmy)tileObject;
                        var neighb = neutralArmy.location.Neighborhood.Inside(Size);
                        foreach (var t in neighb.Select(pt => map[pt.Y, pt.X]))
                            foreach(var to in t.Objects)
                                if (to is CapturableObject)
                                    neutralArmy.GuardObject((CapturableObject)to);
                    }
        }

        public Map(int width, int height, IEnumerable<Tile> tiles)
            : this(width, height)
        {
            foreach (var tile in tiles)
                map[tile.Location.Y, tile.Location.X] = tile;
        }

        public Tile MakeTile(int x, int y, string s)
        {
            var terrain = InitTerrain(char.ToUpper(s[0]));
            var tile = new Tile(x, y, terrain, new List<TileObject>());

            TileObject obj = InitObject(s, new Location(y, x));

            if (obj != null)
            {
                obj.Remove += o => tile.Objects.Remove(o);
                tile.Objects.Add(obj);
            }

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
                case 'A':
                    {
                        var army = CreateArmyFromString(s);
                        return new NeutralArmy(army, location);
                    }
                case 'D':
                    {
                        var recriutTypeName = Enum.GetNames(typeof(UnitType))
                            .SingleOrDefault(res => res[0] == s[2]);
                        var unitType = (UnitType)Enum.Parse(typeof(UnitType), recriutTypeName);
                        return new Dwelling(UnitFactory.CreateFromUnitType(unitType), location);
                    }
                case 'G':
                    {
                        var guards = CreateArmyFromString(s);
                        return new Garrison(guards, location);
                    }
                case '-':
                    return null;
                default:
                    throw new ArgumentException("Unknown object!");
            }
        }

        private Dictionary<UnitType, int> CreateArmyFromString(string s)
        {
            var army = new Dictionary<UnitType, int>();
            var armyS = s.Substring(2).Split('.');
            foreach (var unitS in armyS)
            {
                var unitName = Enum.GetNames(typeof(UnitType))
                    .SingleOrDefault(res => res[0] == unitS[0]);
                var unitType = (UnitType)Enum.Parse(typeof(UnitType), unitName);
                int amount = int.Parse(unitS.Substring(1));
                if (!army.ContainsKey(unitType))
                    army.Add(unitType, 0);
                army[unitType] += amount;
            }
            return army;
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
