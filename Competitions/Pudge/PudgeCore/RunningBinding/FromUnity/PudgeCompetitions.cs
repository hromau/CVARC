using System.Collections.Generic;
using CVARC.V2;
using UnityCommons;
using Infrastructure;

namespace Pudge.RunningBinding.FromUnity
{
    public static class PudgeEngineMaker
    {
        public static List<IEngine> CreateEngines(GameSettings settings)
        {
            var engines = new List<IEngine> { new CommonEngine() };
            engines.Add(new PudgeWorldEngine((CommonEngine)engines[0]));
            return engines;
        }
    }
}
