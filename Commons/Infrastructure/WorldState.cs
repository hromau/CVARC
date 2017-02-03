using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVARC.V2
{
    public class WorldState
    {
        public bool Undefined { get; private set; }

        public static WorldState MakeUndefined() { return new WorldState { Undefined = true }; }
    }
}
