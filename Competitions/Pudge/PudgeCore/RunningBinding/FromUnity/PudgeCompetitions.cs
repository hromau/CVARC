using System.Collections.Generic;
using CVARC.V2;
using UnityCommons;

namespace Pudge.RunningBinding.FromUnity
{
    public static class PudgeEngineMaker
    {
        public static List<IEngine> CreateEngines()
        {
            var engines = new List<IEngine> { new CommonEngine() };
            engines.Add(new PudgeWorldEngine((CommonEngine)engines[0]));
            return engines;
        }
    }
}
