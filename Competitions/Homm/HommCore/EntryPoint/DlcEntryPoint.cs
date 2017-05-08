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
            yield return Competitions(HommLevel.Level1, new HommLogicPartHelper(playersCount: 1));

            yield return Competitions(HommLevel.Level2, new HommLogicPartHelper(playersCount: 2));

            yield return Competitions(HommLevel.Level3, new HommLogicPartHelper(playersCount: 2, limitView: true));

            yield return Competitions(HommLevel.Final, new HommLogicPartHelper(playersCount: 2, limitView: true, useRoughQuantities:true));
        }

        private static Competitions Competitions(HommLevel level, HommLogicPartHelper logicPartHelper)
        {
            return new Competitions("homm", level.ToString(), logicPartHelper, () => new UKeyboard(), CreateEngines);
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
