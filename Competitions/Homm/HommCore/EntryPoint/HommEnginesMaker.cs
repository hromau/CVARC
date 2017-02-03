using System.Collections.Generic;
using CVARC.V2;
using UnityCommons;
using HoMM.Engine;

namespace HoMM
{
    public static class HommEnginesMaker
    {
        public static List<IEngine> CreateEngines()
        {
            return new List<IEngine> { new CommonEngine(), new HommEngine() };
        }
    }
}
