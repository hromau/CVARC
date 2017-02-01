using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Generators
{
    public class SpawnerConfig
    {
        public Location EmitterLocation { get; }
        public double StartRadius { get; }
        public double EndRadius { get; }
        public double SpawnDensity { get; }
        public double SpawnDistance => 1 / SpawnDensity;

        public SpawnerConfig(Location emitter, double startInclusive, double endExclusive, double density)
        {
            if (density > 1 || density <= 0)
                throw new ArgumentException($"{nameof(density)} should be in range (0, 1]");

            EmitterLocation = emitter;
            StartRadius = startInclusive;
            EndRadius = endExclusive;
            SpawnDensity = density;
        }
    }

}
