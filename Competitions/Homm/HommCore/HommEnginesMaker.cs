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
            var commonEngine = new CommonEngine();
            var hommEngine = new HommEngine(commonEngine);
            return new List<IEngine> { commonEngine, hommEngine };
        }
    }
}
