using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Generators
{
    public class RandomSpawner : ISpawner
    {
        private readonly Random random;
        private readonly SpawnerConfig config;

        private readonly Func<ISigmaMap<List<TileObject>>, ISigmaMap<MazeCell>, Location, TileObject> factory;
        private readonly Func<TileObject, Location, TileObject> symmetricFactory;
        private readonly Func<ISigmaMap<List<TileObject>>, ISigmaMap<MazeCell>, IEnumerable<Location>> getSpawnLocations;

        public RandomSpawner(Random random,
            SpawnerConfig config,
            Func<ISigmaMap<List<TileObject>>, ISigmaMap<MazeCell>, Location, TileObject> factory,
            Func<TileObject, Location, TileObject> symmetricFactory,
            Func<ISigmaMap<List<TileObject>>, ISigmaMap<MazeCell>, IEnumerable<Location>> spawnLocations)
        {
            this.random = random;
            this.factory = factory;
            this.config = config;
            this.symmetricFactory = symmetricFactory;
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

            var spawned = new Dictionary<Location, TileObject>();

            return SparseSigmaMap.From(maze.Size,
                s => IsSpawnPoint(spawnPoints, s, maze.Size) ? SpawnObject(spawned, map, maze, s) : null);
        }

        private TileObject SpawnObject(Dictionary<Location, TileObject> spawnedObjects,
            ISigmaMap<List<TileObject>> map, ISigmaMap<MazeCell> maze, Location location)
        {
            var mirror = location.DiagonalMirror(map.Size);

            if (spawnedObjects.ContainsKey(mirror) && symmetricFactory != null)
                return symmetricFactory(spawnedObjects[mirror], location);

            var spawnedObject = factory(map, maze, location);

            spawnedObjects[location] = spawnedObject;

            return spawnedObject;
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
