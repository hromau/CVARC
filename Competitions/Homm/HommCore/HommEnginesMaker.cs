using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
