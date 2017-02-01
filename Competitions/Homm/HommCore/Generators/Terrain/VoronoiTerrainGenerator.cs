using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HoMM.Generators
{
    public class VoronoiTerrainGenerator : RandomGenerator, ITerrainGenerator
    {
        private TileTerrain[] terrains;

        public VoronoiTerrainGenerator(Random random, params TileTerrain[] terrains) 
            : base(random)
        {
            this.terrains = terrains;
        }

        public ISigmaMap<TileTerrain> Construct(ISigmaMap<MazeCell> maze)
        {
            var voronoiMap = new VoronoiMap<TileTerrain>(maze.Size, terrains, random);
            return new ArraySigmaMap<TileTerrain>(maze.Size, i => voronoiMap[i]);
        }
    }
}
