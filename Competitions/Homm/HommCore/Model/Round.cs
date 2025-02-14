﻿using CVARC.V2;
using Infrastructure;
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

        public Round(string filename, WorldClocks clocks, params string[] playerNames)
        {
            Map = new Map(filename);
            Players = playerNames.Select(name => new Player(name, Map, clocks)).ToArray();
        }

        public void UpdateTick(params Location[] playerPositions)
        {
            if (playerPositions.Length != Players.Length)
                throw new ArgumentException("wrong number of player positions!");

            for (int i = 0; i < Players.Length; i++)
                if (Players[i].Location != playerPositions[i])
                    Update(Players[i], playerPositions[i]);
        }

        public void Update(Player player, Location newLocation)
        {
            if (!Players.Contains(player))
                throw new ArgumentException($"{nameof(player)} is not playing this round!");

            player.Location = newLocation;

            // need copy list due to exception when collection is modified inside cycle
            var interactableObjects = Map[newLocation].Objects.ToList();

            foreach (var tileobject in interactableObjects)
                tileobject.InteractWithPlayer(player);
        }

        public void DailyTick()
        {
            Debugger.Log("Daily tick");

            foreach (var tile in Map)
                foreach (var mine in tile.Objects.Where(x => x is Mine).Cast<Mine>())
                {
                    mine.Owner?.GainResources(mine.Resource, mine.Yield);
                    mine.Owner?.OnMineOwnForOneDay(mine);
                }

            DaysPassed++;
            if (DaysPassed % 7 == 0)
                WeeklyTick();

            Debugger.Log("DailyTick done!");
        }

        private void WeeklyTick()
        {
            Debugger.Log("Weekly tick");

            var dwellings = Map
                .SelectMany(t => t.Objects)
                .Where(obj => obj is Dwelling)
                .Cast<Dwelling>();

            foreach (var dwelling in dwellings)
                dwelling.AddWeeklyGrowth();
        }
    }
}
