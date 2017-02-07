using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Generators
{
    public class SimpleSpawner : ISpawner
    {
        Func<ISigmaMap<MazeCell>, IEnumerable<Location>> locations;
        Func<ISigmaMap<List<TileObject>>, ISigmaMap<MazeCell>, Location, TileObject> factory;

        public SimpleSpawner(
            Func<ISigmaMap<List<TileObject>>, ISigmaMap<MazeCell>, Location, TileObject> factory, 
            Func<ISigmaMap<MazeCell>, IEnumerable<Location>> locations)
        {
            this.factory = factory;
            this.locations = locations;
        }

        public ISigmaMap<TileObject> Spawn(ISigmaMap<List<TileObject>> map, ISigmaMap<MazeCell> maze)
        {
            var locationsToSpawn = new HashSet<Location>(locations(maze));

            return SparseSigmaMap.From(map.Size,
                loc => locationsToSpawn.Contains(loc) ? factory(map, maze, loc) : null);
        }
    }
}
