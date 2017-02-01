using HoMM.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.World
{
    static class MapHelper
    {
        public static Map CreateMap(Random random)
        {
            var easyTier = new SpawnerConfig(Location.Zero, 3, 30, 0.5);
            var mediumTier = new SpawnerConfig(Location.Zero, 30, 1000, 0.5);
            var hardTier = new SpawnerConfig(Location.Zero, 14, 16, 0.5);
            var nightmare = new SpawnerConfig(Location.Zero, 16.5, 20, 0.5);

            var mapGenerator = HommMapGenerator
                .From(new DiagonalMazeGenerator(random))
                .With(new BfsRoadGenerator(random, TileTerrain.Road)
                    .Over(new VoronoiTerrainGenerator(random, TileTerrain.Nature.ToArray())))
                .With(new GraphSpawner(random, mediumTier, p => new Mine(Resource.Crystals, p)))
                .With(new DistanceSpawner(random, hardTier, p => new Mine(Resource.Ore, p)))
                .With(new GraphSpawner(random, easyTier, p => new Mine(Resource.Rubles, p)))
                .And(new DistanceSpawner(random, nightmare, p => new Mine(Resource.Gems, p)));

            return mapGenerator.GenerateMap(18);
        }
    }
}
