using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Generators
{
    public class RandomSpawner : ISpawner
    {
        private readonly Random random;
        private readonly Func<Location, TileObject> factory;
        private readonly SpawnerConfig config;

        private readonly Func<ISigmaMap<List<TileObject>>, ISigmaMap<MazeCell>, IEnumerable<Location>> getSpawnLocations;

        public RandomSpawner(Random random,
            SpawnerConfig config,
            Func<Location, TileObject> factory,
            Func<ISigmaMap<List<TileObject>>, ISigmaMap<MazeCell>, IEnumerable<Location>> spawnLocations)
        {
            this.random = random;
            this.factory = factory;
            this.config = config;
            getSpawnLocations = spawnLocations;
        }

        public ISigmaMap<TileObject> Spawn(ISigmaMap<List<TileObject>> map, ISigmaMap<MazeCell> maze)
        {
            var potentialLocations = getSpawnLocations(map, maze).ToArray();
            
            var spawnPoints = new HashSet<Location>();

            while (potentialLocations.Length != 0)
            {
                var i = random.Next(potentialLocations.Length);

                var spawnPoint = potentialLocations[i];
                spawnPoints.Add(spawnPoint);

                potentialLocations = potentialLocations
                    .Where(s => s.EuclideanDistance(spawnPoint) >= config.SpawnDistance)
                    .ToArray();
            }

            return SparseSigmaMap.From(maze.Size,
                s => IsSpawnPoint(spawnPoints, s, maze.Size) ? factory(s) : null);
        }

        private bool IsSpawnPoint(HashSet<Location> spawns, Location location, MapSize size)
        {
            if (!IsNearBorder(location, size))
                return spawns.Contains(location.AboveDiagonal(size));

            if (location.X >= size.X / 2.0)
                return spawns.Contains(location);

            return location.AboveDiagonal(size) == location 
                ? false
                : spawns.Contains(location.AboveDiagonal(size));
        }

        private bool IsNearBorder(Location location, MapSize size)
        {
            return location.AboveDiagonal(size).ManhattanDistance(Location.Zero) > size.X - 2;
        }
    }
}
