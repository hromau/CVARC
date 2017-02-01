using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Generators
{
    public class BfsRoadGenerator : RandomGenerator, ITerrainGenerator
    {
        private TileTerrain roadTile;

        public BfsRoadGenerator(Random random, TileTerrain roadTile)
            : base(random)
        {
            this.roadTile = roadTile;
        }
        
        public ISigmaMap<TileTerrain> Construct(ISigmaMap<MazeCell> maze)
        {
            var road = FindRoad(Location.Zero, new Location(maze.Size.X - 1, maze.Size.Y - 1), maze);
            
            return new ArraySigmaMap<TileTerrain>(maze.Size, 
                i => road.Contains(i.AboveDiagonal(maze.Size)) ? roadTile : null);
        }

        private HashSet<Location> FindRoad(Location from, Location to, ISigmaMap<MazeCell> maze)
        {
            return new HashSet<Location>(Graph.BreadthFirstSearch(from,
                    s => s.Neighborhood.Where(n => n.IsInside(maze.Size) && maze[n] == MazeCell.Empty),
                    s => s == to)
                .Select(x => x.AboveDiagonal(maze.Size)));
        }
    }
}
