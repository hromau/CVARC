using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pudge.World
{
    class DebugRuneRespawnTrigger : RuneRespawnTrigger
    {
        public DebugRuneRespawnTrigger(PudgeWorld world)
            : base(world, double.PositiveInfinity)
        { }

        protected override void RespawnRunes()
        {
            SpawnRune(-60, -150, RuneType.Haste);
            SpawnRune(-30, -150, RuneType.GoldXP);
            SpawnRune(0, -150, RuneType.Invisibility);

            SpawnRune(60, -150, RuneType.Haste, RuneSize.Large);
            SpawnRune(90, -150, RuneType.GoldXP, RuneSize.Large);
            SpawnRune(120, -150, RuneType.Invisibility, RuneSize.Large);
        }
    }
}
