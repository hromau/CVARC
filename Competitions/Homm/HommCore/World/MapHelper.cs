using HoMM.Generators;
using System;
using System.Linq;

namespace HoMM.World
{
    public class MapHelper
    {
        const int mapSize = 18;

        public Map CreateMap(Random r) => CreateGenerator(r).GenerateMap(mapSize);

        public HommMapGenerator CreateGenerator(Random r)
        {
            var builder = HommMapGenerator
                .From(new DiagonalMazeGenerator(r))
                .With(new BfsRoadGenerator(r, TileTerrain.Road)
                    .Over(new VoronoiTerrainGenerator(r, TileTerrain.Nature.ToArray())));

            builder = AddGuards(builder, r);
            builder = AddPiles(builder, r);
            builder = AddMines(builder, r);
            builder = AddDwellings(builder, r);
            builder = AddEnemies(builder, r);

            return builder.CreateGenerator();
        }

        private Resource[] spawnableResources =
            new[] { Resource.Ore, Resource.Wood, Resource.Gold, Resource.Horses };

        private UnitType[] spawnableDwellings =
            new[] { UnitType.Cavalry, UnitType.Infantry, UnitType.Ranged };

        private HommMapGenerator.BuilderOnSelectEntities AddDwellings(
            HommMapGenerator.BuilderOnSelectEntities builder, Random random)
        {
            var dwellingsConfig = new SpawnerConfig(Location.Zero, 30, 100, 0.2);

            foreach (var unitType in spawnableDwellings)
                builder = builder.With(new GraphSpawner(random, dwellingsConfig,
                    p => new Dwelling(UnitFactory.CreateFromUnitType(unitType), p)));

            return builder;
        }

        private HommMapGenerator.BuilderOnSelectEntities AddMines(
            HommMapGenerator.BuilderOnSelectEntities builder, Random random)
        {
            var minesConfig = new SpawnerConfig(Location.Zero, 40, 100, 0.2);

            foreach (var resource in spawnableResources)
                builder = builder.With(new GraphSpawner(random, minesConfig, p => new Mine(resource, p)));

            return builder;
        }

        private HommMapGenerator.BuilderOnSelectEntities AddPiles(
            HommMapGenerator.BuilderOnSelectEntities builder, Random random)
        {
            var nearPiles = new SpawnerConfig(Location.Zero, 3, 25, 0.3);
            var farPiles = new SpawnerConfig(Location.Zero, 25, 100, 0.1);

            foreach (var resource in spawnableResources)
            {
                builder = builder
                    .With(new GraphSpawner(random, nearPiles, l => new ResourcePile(resource, 10, l)))
                    .With(new GraphSpawner(random, farPiles, l => new ResourcePile(resource, 25, l)));
            }

            return builder;
        }

        private HommMapGenerator.BuilderOnSelectEntities AddGuards(
            HommMapGenerator.BuilderOnSelectEntities gen, Random random)
        {
            var guardsConfig = new SpawnerConfig(Location.Zero, 16.5, 100, 1);
            return gen.With(new DistanceSpawner(random, guardsConfig, p => NeutralArmy.BuildRandom(p, 40, 50)));
        }

        private HommMapGenerator.BuilderOnSelectEntities AddEnemies(
            HommMapGenerator.BuilderOnSelectEntities gen, Random random)
        {
            var easyTier = new SpawnerConfig(Location.Zero, 0, 70, 0.6);
            var hardTier = new SpawnerConfig(Location.Zero, 70, 100, 0.6);

            return gen

                .With(new GraphSpawner(random, easyTier, p => NeutralArmy.BuildRandom(p, 5, 10),
                    (map, maze, s) => map[s] is Mine))

                .With(new GraphSpawner(random, hardTier, p => NeutralArmy.BuildRandom(p, 10, 30),
                    (map, maze, s) => map[s] is Mine));
        }
    }
}
