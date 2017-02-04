using System;
using System.Collections.Generic;
using System.Linq;

namespace HoMM
{
    public class Round
    {
        public Map Map { get; }
        public Player[] Players { get; }
        public int DaysPassed { get; private set; } = 0;

        public Round(Map map, params Player[] players)
        {
            Map = map;
            Players = players;
        }

        public Round(string filename, params string[] playerNames)
        {
            Map = new Map(filename);
            Players = playerNames.Select(name => new Player(name, Map)).ToArray();
        }

        public void UpdateTick(params Location[] playerPositions)
        {
            if (playerPositions.Length != Players.Length)
                throw new ArgumentException("wrong number of player positions!");

            for (int i = 0; i < Players.Length; i++)
                Update(Players[i], playerPositions[i]);
        }

        public void Update(Player player, Location newLocation)
        {
            if (!Players.Contains(player))
                throw new ArgumentException($"{nameof(player)} is not playing this round!");

            if (player.Location == newLocation)
                return;

            player.Location = newLocation;

            foreach(var tileobject in Map[newLocation].Objects)
                tileobject?.InteractWithPlayer(player);
        }

        public void DailyTick()
        {
            foreach (var tile in Map)
                foreach (var obj in tile.Objects)
                    if (obj is Mine)
                    {
                        var m = obj as Mine;
                        if (m.Owner != null)
                            m.Owner.GainResources(m.Resource, m.Yield);
                    }

            DaysPassed++;
            if (DaysPassed % 7 == 0)
                WeeklyTick();
        }

        private void WeeklyTick()
        {
            var dwellings = Map
                .SelectMany(t => t.Objects)
                .Where(obj => obj is Dwelling)
                .Cast<Dwelling>();

            foreach (var dwelling in dwellings)
                dwelling.AddWeeklyGrowth();
        }
    }
}
