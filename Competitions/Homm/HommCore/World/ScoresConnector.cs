using CVARC.V2;
using HoMM.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.World
{
    static class ScoresConnector
    {
        public static void Connect(Scores scoreboard, Player player)
        {
            var r = HommRules.Current;
            var id = player.Name;

            player.MineCaptured += _ => scoreboard.Add(id, r.MineCaptureScores, "Mine has been captured");
            player.DwellingCaptured += _ => scoreboard.Add(id, r.DwellingCaptureScores, "Dwelling has been captured");

            player.ResourcesGained += (res, count) => scoreboard.Add(id, r.ResourcesGainScores * count, $"Got {res}");

            player.VictoryAchieved += (opponent, army) =>
            {
                if (opponent is Player)
                    scoreboard.Add(id, r.OtherPlayerDefeatScores + GetArmyDefeatBonus(army), "Achieved victory in combat against player's army");
                if (opponent is NeutralArmy)
                    scoreboard.Add(id, r.NeutralArmyDefeatScores + GetArmyDefeatBonus(army), "Achieved victory in combat against neutral army");
                if (opponent is Garrison)
                    scoreboard.Add(id, r.GarrisonDefeatScores + GetArmyDefeatBonus(army), "Achieved victory in combat against garrison");
            };
        }

        private static int GetArmyDefeatBonus(Dictionary<UnitType, int> army)
        {
            return army.Sum(kv => HommRules.Current.UnitDefeatScores[kv.Key] * kv.Value);
        }
    }
}
