using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.World
{
    [Serializable]
    public class HommWorldState : WorldState
    {
        public int Seed { get; }
        public bool Debug { get; }

        public HommWorldState(int seed, bool debug=false)
        {
            Seed = seed;
            Debug = debug;
        }
    }
}
