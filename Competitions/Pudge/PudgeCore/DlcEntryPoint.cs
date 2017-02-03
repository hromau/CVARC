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
            for (var i = 1; i < 3; i++) // сейчас есть только 2 уровня
            {
                var levelName = string.Format(LevelNameTemplate, i);
                yield return new Competitions("Pudge", levelName, new ReleaseLogicPartHelper(i),
                    () => new UKeyboard(), PudgeEngineMaker.CreateEngines);
            }
        }
    }
}
