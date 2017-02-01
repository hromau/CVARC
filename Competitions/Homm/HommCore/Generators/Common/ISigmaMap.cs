using System.Collections.Generic;

namespace HoMM.Generators
{
    public interface ISigmaMap<TCell> : IEnumerable<Location>
    {
        MapSize Size { get; }
        TCell this[Location location] { get; }
    }
}
