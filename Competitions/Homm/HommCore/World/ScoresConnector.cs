using CVARC.V2;
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

            player.UnitsAdded += (unit, count) => scoreboard.Add(id, r.Units.Scores[unit], $"War: {count} {unit} {(count == 1 ? "is" : "are")} hired.");

            player.ResourcesGained += (res, count) => scoreboard.Add(id, r.ResourcesGainScores*count, $"Economics: got {count} {res}.");

            player.OwnMineForHour += _ => scoreboard.Add(id, r.MineOwningScores, "Domination: own mine");

            player.VictoryAchieved += (opponent, army) => scoreboard.Add(id, GetArmyDefeatBonus(army), "War: victory is achieved in combat.");
        }

        private static int GetArmyDefeatBonus(Dictionary<UnitType, int> army)
        {
            return army.Sum(kv => HommRules.Current.Units.Scores[kv.Key] * kv.Value);
        }
    }
}
