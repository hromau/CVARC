﻿using HoMM.ClientClasses;
using System;
using System.Collections.Generic;

namespace HoMM
{
    public class Garrison : CapturableObject, ICombatable
    {
        public override bool IsPassable => true;

        public Dictionary<UnitType, int> Army { get; private set; }
        public Garrison(Dictionary<UnitType, int> guards, Location location, Player owner) : base(location)
        {
            Army = guards;
            Owner = owner;
        }

        public void Populate(Dictionary<UnitType, int> additionalArmy)
        {
            foreach (var kv in additionalArmy)
            {
                var unitType = kv.Key;
                var count = kv.Value;

                Army[unitType] = Army.GetOrDefault(unitType, 0) + count;
            }
        }

        public override void InteractWithPlayer(Player p)
        {
            if (p != Owner)
            {
                Combat.Resolve(p, this);
                if (this.HasNoArmy())
                    Owner = p;
            }
        }

        public void SetUnitsCount(UnitType unitType, int count)
        {
            Army[unitType] = count;
        }
    }
}
