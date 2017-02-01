using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Generators
{
    class EmptySigmaMap<TCell> : ImmutableSigmaMap<TCell>
    {
        public override TCell this[Location location]
        {
            get { return default(TCell); }
        }

        public override MapSize Size { get; }

        public EmptySigmaMap(MapSize size)
        {
            Size = size;
        }
    }

    static class SigmaMap
    {
        public static ISigmaMap<TCell> Empty<TCell>(MapSize size)
        {
            return new EmptySigmaMap<TCell>(size);
        }
    }
}
