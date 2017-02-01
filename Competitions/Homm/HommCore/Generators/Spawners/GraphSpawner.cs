using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Generators
{
    public class GraphSpawner : RandomSpawner
    {
        public GraphSpawner(
            Random random,
            SpawnerConfig config,
            Func<Location, TileObject> factory)

            : base(random, config, factory,
                  maze => Graph.BreadthFirstTraverse(Location.Zero, s => s.Neighborhood
                        .InsideAndAboveDiagonal(maze.Size)
                        .Where(n => maze[n] == MazeCell.Empty))
                    .Select((x, i) => new { Distance = i, Node = x.Node })
                    .SkipWhile(x => x.Distance < config.StartRadius)
                    .TakeWhile(x => x.Distance < config.EndRadius)
                    .Select(x => x.Node))
        { }
    }

}
