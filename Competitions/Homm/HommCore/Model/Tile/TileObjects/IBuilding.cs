using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM
{
    public interface IBuilding
    {
        Location BuildingLocation { get; }
        Location EntryLocation { get; }
    }
}
