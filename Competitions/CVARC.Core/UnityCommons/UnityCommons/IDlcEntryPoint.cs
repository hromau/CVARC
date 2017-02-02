using System.Collections.Generic;
using CVARC.V2;

namespace UnityCommons
{
    public interface IDlcEntryPoint
    {
        IEnumerable<Competitions> GetLevels();
    }
}
