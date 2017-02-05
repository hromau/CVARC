using System;
using CVARC.V2;

namespace Pudge.World
{
    [Serializable]
    public class PudgeWorldState : WorldState
    {
        public int Seed;
        public PudgeWorldState(int seed)
        {
            Seed = seed;
        }
    }
}
