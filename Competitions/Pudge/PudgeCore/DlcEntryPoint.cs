using Pudge.RunningBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityCommons;
using CVARC.V2;
using Pudge.RunningBinding.FromUnity;
using UnityEngine;

namespace Pudge
{
    public class DlcEntryPoint : IDlcEntryPoint
    {
        private const string LevelNameTemplate = "Level{0}";

        public string FullCompetitionsName { get { return "Pudge Wars"; } }

        public Texture MenuBackground { get { return AssetLoader.LoadAsset<Texture>("pudge", "Background.png"); } }

        public IEnumerable<Competitions> GetLevels()
        {
            //TODO: Здесь какая-то херня. Как вообще вызывать правила 1 и 2 недели?
            yield return new Competitions("pudge", "final", new ReleaseLogicPartHelper(2), () => new UKeyboard(), PudgeEngineMaker.CreateEngines);
            yield return new Competitions("pudge", "debug", new DebugLogicPartHelper(), () => new UKeyboard(), PudgeEngineMaker.CreateEngines);
        }
    }
}
