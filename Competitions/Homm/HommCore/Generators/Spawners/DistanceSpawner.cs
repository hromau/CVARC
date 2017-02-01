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
            Func<Location, TileObject> factory)

            : base(random, config, factory,
                  maze => Location.Square(maze.Size)
                    .Where(s => maze[s] == MazeCell.Empty)
                    .Select(s => new { Value = s, Distance = config.EmitterLocation.ManhattanDistance(s) })
                    .Where(s => s.Distance >= config.StartRadius)
                    .Where(s => s.Distance < config.EndRadius)
                    .Select(s => s.Value)
                    .Where(s => s.IsAboveDiagonal(maze.Size)))
        { }
    }
}
