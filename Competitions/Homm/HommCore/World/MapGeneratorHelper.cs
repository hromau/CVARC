using HoMM.Generators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HoMM.World
{
    public partial class MapGeneratorHelper
    {
        const int mapSize = 14;

        public Map CreateMap(Random r)
        {
            HommMapGenerator generator = CreateGenerator(r);

            Map map = null;

            while (map == null)
            {
                try { map = generator.GenerateMap(mapSize); }
                catch { }
            }

            return map;
        }

        public HommMapGenerator CreateGenerator(Random r)
        {
            var builder = HommMapGenerator
                .From(new DiagonalMazeGenerator(r))
                .With(new BfsRoadGenerator(r, TileTerrain.Road)
                    .Over(new VoronoiTerrainGenerator(r, TileTerrain.Nature.ToArray())));

            builder = AddGuards(builder, r);
            builder = AddMines(builder, r);
            builder = AddDwellings(builder, r);
            builder = AddPiles(builder, r);
            builder = AddEnemies(builder, r);

            return builder.CreateGenerator();
        }

        private Resource[] spawnableResources =
            new[] { Resource.Iron, Resource.Glass, Resource.Gold, Resource.Ebony };

        private UnitType[] spawnableDwellings =
            new[] { UnitType.Cavalry, UnitType.Infantry, UnitType.Ranged };

        private HommMapGenerator.BuilderOnSelectEntities AddDwellings(
            HommMapGenerator.BuilderOnSelectEntities builder, Random random)
        {
            builder = builder
                .With(new SimpleSpawner(
                    (map, maze, location) => new Dwelling(UnitType.Militia,
                        location, location == Location.Zero ? location - Vector2i.One : location + Vector2i.One,
                        HommRules.Current.Units.WeeklyGrowth[UnitType.Militia]),
                    maze => new[] { Location.Zero, Location.Max(maze.Size) }));

            var dwellingsConfig = new SpawnerConfig(Location.Zero, 15, 100, 0.2);

            foreach (var unitType in spawnableDwellings)
                builder = builder.With(new GraphSpawner(random, dwellingsConfig,
                    (map, maze, loc) => new Dwelling(unitType, loc,
                        FindPlaceForBuilding(map, maze, loc), HommRules.Current.Units.WeeklyGrowth[unitType]),
                    (map, maze, loc) => IsGoodPlaceForBuilding(map, maze, loc)));

            return builder;
        }

        private HommMapGenerator.BuilderOnSelectEntities AddMines(
            HommMapGenerator.BuilderOnSelectEntities builder, Random random)
        {
            var minesConfig = new SpawnerConfig(Location.Zero, 25, 100, 0.2);

            foreach (var resource in spawnableResources)
            {
                builder = builder.With(new GraphSpawner(random, minesConfig,
                    (map, maze, loc) => new Mine(resource, loc, FindPlaceForBuilding(map, maze, loc)),
                    (map, maze, loc) => IsGoodPlaceForBuilding(map, maze, loc)));
            }

            return builder;
        }

        private bool IsGoodPlaceForBuilding(
            ISigmaMap<List<TileObject>> map, ISigmaMap<MazeCell> maze, Location placeToCheck)
        {
            var isWall = maze[placeToCheck] == MazeCell.Wall;
            var isOccupiedByOther = map[placeToCheck] != null && map[placeToCheck].Count != 0;
            var hasPlaceForBuilding = FindPlaceForBuilding(map, maze, placeToCheck) != null;

            return !isWall && !isOccupiedByOther && hasPlaceForBuilding;
        }

        private Location FindPlaceForBuilding(
            ISigmaMap<List<TileObject>> map, ISigmaMap<MazeCell> maze, Location location)
        {
            Func<Location, IEnumerable<Location>> neighbors = loc => loc.Neighborhood.Inside(map.Size);

            Func<Location, bool> isBuilding = loc => neighbors(loc)
                .Any(neighbor => map[neighbor].Any(x => (x is IBuilding && ((IBuilding)x).BuildingLocation == loc)));

            return neighbors(location)
                .Where(neighbor => maze[neighbor] == MazeCell.Wall && !isBuilding(neighbor))
                .FirstOrDefault();
        }

        private HommMapGenerator.BuilderOnSelectEntities AddPiles(
            HommMapGenerator.BuilderOnSelectEntities builder, Random random)
        {
            var nearPiles = new SpawnerConfig(Location.Zero, 3, 15, 0.3);
            var farPiles = new SpawnerConfig(Location.Zero, 15, 100, 0.1);

            foreach (var resource in spawnableResources)
            {
                builder = builder
                    
                    .With(new GraphSpawner(random, nearPiles, 
                        (map, maze, location) => new ResourcePile(resource, 10, location)))
                    
                    .With(new GraphSpawner(random, farPiles, 
                        (map, maze, location) => new ResourcePile(resource, 25, location)));
            }

            return builder;
        }

        private HommMapGenerator.BuilderOnSelectEntities AddGuards(
            HommMapGenerator.BuilderOnSelectEntities builder, Random random)
        {
            var guardsConfig = new SpawnerConfig(Location.Zero, 12.5, 100, 1);
            return builder.With(new DistanceSpawner(random, guardsConfig,
                (map, maze, p) => NeutralArmy.BuildRandom(p, 400, random),
                symmetricFactory: (original, location) => ((NeutralArmy)original).Copy(location)));
        }

        private HommMapGenerator.BuilderOnSelectEntities AddEnemies(
            HommMapGenerator.BuilderOnSelectEntities builder, Random random)
        {
            var easyTier = new SpawnerConfig(Location.Zero, 0, 40, 1);
            var hardTier = new SpawnerConfig(Location.Zero, 40, 100, 1);

            return builder

                .With(new GraphSpawner(random, easyTier,
                    (map, maze, location) => NeutralArmy.BuildRandom(location, 75, random),
                    (map, maze, location) => map[location].Any(x => x is Mine),
                    (original, location) => ((NeutralArmy)original).Copy(location)))

                .With(new GraphSpawner(random, hardTier, 
                    (map, maze, location) => NeutralArmy.BuildRandom(location, 225, random),
                    (map, maze, location) => map[location].Any(x => x is Mine),
                    (original, location) => ((NeutralArmy)original).Copy(location)));
        }
    }
}
