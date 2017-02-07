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
            Func<ISigmaMap<List<TileObject>>, ISigmaMap<MazeCell>, Location, TileObject> factory,
            Func<ISigmaMap<List<TileObject>>, ISigmaMap<MazeCell>, Location, bool> predicate = null,
            Func<TileObject, Location, TileObject> symmetricFactory = null)

            : base(random, config, factory, symmetricFactory,
                  (map, maze) => Graph.BreadthFirstTraverse(Location.Zero, s => s.Neighborhood
                        .InsideAndAboveDiagonal(maze.Size)
                        .Where(n => maze[n] == MazeCell.Empty))
                    .Select((x, i) => new { Distance = i, Node = x.Node })
                    .SkipWhile(x => x.Distance < config.StartRadius)
                    .TakeWhile(x => x.Distance < config.EndRadius)
                    .Select(x => x.Node)
                    .Where(s => predicate?.Invoke(map, maze, s) ?? (map[s] == null || map[s].Count == 0)))
        { }
    }

}
