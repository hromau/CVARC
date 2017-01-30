using System.Collections.Generic;
using CVARC.V2;
using Pudge.RunningBinding.FromUnity;
using UnityCommons;
using UnityEngine;

namespace Pudge.RunningBinding
{
    public class BundleEntryPoint //: IBundleEntryPoint
    {
        private const string LevelNameTemplate = "Level{0}";

        public IEnumerable<Competitions> GetLevels()
        {
            // тк сейчас есть только 2 уровня
            for (var i = 1; i < 3; i++)
            {
                var levelName = string.Format(LevelNameTemplate, i);
                yield return new Competitions(
                    levelName, new ReleaseLogicPartHelper(i), () => new UKeyboard(), PudgeEngineMaker.CreateEngines);
            }
        }
    }
}
