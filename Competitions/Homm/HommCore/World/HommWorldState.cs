using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.World
{
    [Serializable]
    class HommWorldState : IWorldState
    {
        public int Seed { get; }

        public HommWorldState(int seed)
        {
            Seed = seed;
        }
    }
}
