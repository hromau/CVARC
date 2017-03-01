﻿using CVARC.V2;
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

            player.UnitsAdded += (unit, count) => 
                scoreboard.Add(id, r.Units.Scores[unit]*count, $"{count} {unit} {(count == 1 ? "is" : "are")} hired.", "Military");

            player.ResourcesGained += (res, count) => 
                scoreboard.Add(id, r.ResourcesGainScores, $"Got {count} {res}.", "Economics");

            player.OwnMineForOneDay += _ => 
                scoreboard.Add(id, r.MineOwningScores, "Mine is owned.", "Domination");

            player.VictoryAchieved += (opponent, army) => 
                scoreboard.Add(id, GetArmyDefeatBonus(army), "Victory is achieved in combat.", "War");


            foreach (var scoreType in new[] { "Military", "Economics", "Domination", "War" })
                scoreboard.Add(id, 0, "Initial score", scoreType);
        }

        private static int GetArmyDefeatBonus(Dictionary<UnitType, int> army)
        {
            return army.Sum(kv => HommRules.Current.Units.Scores[kv.Key] * kv.Value);
        }
    }
}
