using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Generators
{
    class AggregatedTerrainGenerator : ITerrainGenerator
    {
        private ITerrainGenerator[] generators;

        public AggregatedTerrainGenerator(params ITerrainGenerator[] generators)
        {
            this.generators = generators;
        }

        public ISigmaMap<TileTerrain> Construct(ISigmaMap<MazeCell> maze)
        {
            var terrains = generators
                .Select(g => g.Construct(maze))
                .ToArray();

            return new ArraySigmaMap<TileTerrain>(maze.Size, 
                s => terrains.Select(t => t[s]).FirstOrDefault(t => t != null));
        }
    }
}
