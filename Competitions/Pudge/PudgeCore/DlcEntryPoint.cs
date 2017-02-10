using Pudge.RunningBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityCommons;
using CVARC.V2;
using Pudge.RunningBinding.FromUnity;

namespace Pudge
{
    public class DlcEntryPoint : IDlcEntryPoint
    {
        private const string LevelNameTemplate = "Level{0}";

        public IEnumerable<Competitions> GetLevels()
        {
            //TODO: Здесь какая-то херня. Как вообще вызывать правила 1 и 2 недели?
            yield return new Competitions("Pudge", "Final", new ReleaseLogicPartHelper(2), () => new UKeyboard(), PudgeEngineMaker.CreateEngines);
            yield return new Competitions("Pudge", "Debug", new DebugLogicPartHelper(), () => new UKeyboard(), PudgeEngineMaker.CreateEngines);
        }
    }
}
