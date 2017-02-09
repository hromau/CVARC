using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.World
{
    public partial class MapGeneratorHelper
    {
        public Map CreateDebugMap(Random r)
        {
            var mapSize = new MapSize(8, 8);

            var natureTiles = TileTerrain.Nature.ToArray();

            var terrainSize = 3;

            var map = new Map(mapSize.X, mapSize.Y, Location.Square(mapSize)
                .Select(p => p.Y >= mapSize.Y - terrainSize && p.X < 10
                    ? new Tile(p, natureTiles[p.X / 2], new List<TileObject>())
                    : new Tile(p, TileTerrain.Road, new List<TileObject>())));

            AddBuildings(map, (x, i) => new Dwelling((UnitType)i, x, x + new Vector2i(0, 1), 1000),
                Enumerable.Range(2, 4).Select(x => new Location(4, x)));

            AddBuildings(map, (x, i) => new Mine((Resource)i, x, x + new Vector2i(1, 0)),
                Enumerable.Range(1, 4).Select(y => new Location(y, mapSize.X - 2)));

            Add(map, (x, i) => NeutralArmy.BuildRandom(x, (i + 1) * 50, r),
                Enumerable.Range(1, 7).Select(x => new Location(0, x)));

            Add(map, (x, i) => new ResourcePile((Resource)(i % 4), 1000, x),
                Enumerable.Range(2, 6).Select(y => new Location(y, 0))
                .Union(Enumerable.Range(2, 6).Select(y => new Location(y, 1))));

            map[new Location(3, 4)].Objects.Add(new Wall(new Location(3, 4)));

            return map;
        }

        private void Add(Map map, Func<Location, int, TileObject> factory, IEnumerable<Location> locations)
        {
            locations
                .Select((x, i) => factory(x, i))
                .ToList()
                .ForEach(x => map[x.location].Objects.Add(x));
        }

        private void AddBuildings(Map map, Func<Location, int, IBuilding> factory, IEnumerable<Location> locations)
        {
            locations
                .Select((x, i) => factory(x, i))
                .ToList()
                .ForEach(x =>
                {
                    map[x.EntryLocation].Objects.Add((TileObject)x);
                    map[x.BuildingLocation].Objects.Add(new Wall(x.BuildingLocation));
                });
        }
    }
}
