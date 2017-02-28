using CVARC.V2;
using HoMM.Engine;
using Infrastructure;
using System.Collections.Generic;
using UnityCommons;
using System;
using UnityEngine;
using System.Linq;

namespace HoMM
{
    public class DlcEntryPoint : IDlcEntryPoint
    {
        public IEnumerable<Competitions> GetLevels()
        {
            yield return new Competitions("homm", "level1", new HommLogicPartHelper(playersCount: 1),
                () => new UKeyboard(), CreateEngines);

            yield return new Competitions("homm", "level2", new HommLogicPartHelper(playersCount: 2),
                () => new UKeyboard(), CreateEngines);
        }

        private static List<IEngine> CreateEngines(GameSettings settings)
        {
            return new List<IEngine> {
                new CommonEngine(),
                new HommEngine(settings.SpectacularView),
                new HommUserInterfaceEngine(),
            };
        }

        public Texture MenuBackground => AssetLoader.LoadAsset<Texture>("homm", "bg.jpg");

        public string FullCompetitionsName => "Heroes of Might and Magic";
    }
}
