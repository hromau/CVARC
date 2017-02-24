using System.Collections.Generic;
using CVARC.V2;
using UnityEngine;

namespace UnityCommons
{
    public interface IDlcEntryPoint
    {
        Texture MenuBackground { get; }

        string FullCompetitionsName { get; }

        IEnumerable<Competitions> GetLevels();
    }
}
