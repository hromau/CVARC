using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Generators
{
    public class DistanceSpawner : RandomSpawner
    {
        public DistanceSpawner(
            Random random,
            SpawnerConfig config,
            Func<Location, TileObject> factory,
            Func<ISigmaMap<List<TileObject>>, ISigmaMap<MazeCell>, Location, bool> predicate = null)

            : base(random, config, factory,
                  (map, maze) => Location.Square(maze.Size)
                    .Where(s => predicate?.Invoke(map, maze, s) ?? 
                        maze[s] == MazeCell.Empty && map[s] != null && map[s].Count == 0)
                    .Select(s => new { Value = s, Distance = config.EmitterLocation.ManhattanDistance(s) })
                    .Where(s => s.Distance >= config.StartRadius)
                    .Where(s => s.Distance < config.EndRadius)
                    .Select(s => s.Value)
                    .Where(s => s.IsAboveDiagonal(maze.Size)))
        { }
    }
}
