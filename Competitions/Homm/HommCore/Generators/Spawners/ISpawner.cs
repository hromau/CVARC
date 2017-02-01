using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Generators
{
    public interface ISpawner
    {
        ISigmaMap<TileObject> Spawn(ISigmaMap<MazeCell> maze);
    }
}
