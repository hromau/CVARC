using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoMM
{
    public class Player : ICombatable
    {
        public string Name { get; private set; }
        public int Attack { get; private set; }
        public int Defence { get; private set; }
        private Map map;
        Dictionary<Resource, int> resources;
        public Location Location { get; set; }
        public Location DisiredLocation { get; set; }
        public Dictionary<UnitType, int> Army { get; }

        public string UnityId => Name;


        public Player(string name, Map map)
        {
            Name = name;
            resources = new Dictionary<Resource, int>();
            foreach (Resource res in Enum.GetValues(typeof(Resource)))
                resources.Add(res, 0);
            Army = new Dictionary<UnitType, int>();
            foreach (UnitType t in Enum.GetValues(typeof(UnitType)))
                Army.Add(t, 0);
            this.map = map;
            Attack = 1;
            Defence = 1;
        }

        public Player(string name, Map map, int attack, int defence) : this(name, map)
        {
            Attack = attack;
            Defence = defence;
        }

        public int CheckResourceAmount(Resource res)
        {
            return resources[res];
        }
        public Dictionary<Resource, int> CheckAllResources()
        {
            return new Dictionary<Resource, int>(resources);
        }

        public void GainResources(Resource res, int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Cannot 'gain' negative resources!");
            resources[res] += amount;
        }

        public void PayResources(Resource res, int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Cannot 'pay' positive resources!");
            if (amount > resources[res])
                throw new ArgumentException("Not enough " + res.ToString() + " to pay " + amount);
            resources[res] -= amount;
        }


        public void AddUnits(UnitType unitType, int amount)
        {
            if (!Army.ContainsKey(unitType))
                Army.Add(unitType, 0);
            Army[unitType] += amount;
        }

        public bool TryBuyUnits(int unitsToBuy)
        {
            if (unitsToBuy <= 0)
                throw new ArgumentException("Buy positive amounts of units!");

            var dwelling = (Dwelling)map[Location].Objects.Where(x => x is Dwelling).FirstOrDefault();

            if (dwelling == null || dwelling.Owner != this || dwelling.AvailableUnits < unitsToBuy)
                return false;

            foreach (var kvp in dwelling.Recruit.UnitCost)
                if (CheckResourceAmount(kvp.Key) < kvp.Value * unitsToBuy)
                    return false;
            foreach (var kvp in dwelling.Recruit.UnitCost)
                PayResources(kvp.Key, kvp.Value * unitsToBuy);
            AddUnits(dwelling.Recruit.UnitType, unitsToBuy);
            return true;
        }

        //exchange units, positive amounts give to garrison, negative take from garrison
        public bool TryExchangeUnitsWithGarrison(Dictionary<UnitType, int> unitsToExchange)
        {
            var garrison = (Garrison)map[Location].Objects.Where(x => x is Garrison).FirstOrDefault();

            if (garrison == null || garrison.Owner != this)
                return false;

            foreach (var stack in unitsToExchange)
            {
                if (stack.Value >= 0 && Army[stack.Key] >= stack.Value) 
                {
                    if (!garrison.Army.ContainsKey(stack.Key))
                        garrison.Army.Add(stack.Key, 0);
                    garrison.Army[stack.Key] += stack.Value;
                    Army[stack.Key] -= stack.Value;
                }
                else if (stack.Value < 0 && garrison.Army.ContainsKey(stack.Key) && garrison.Army[stack.Key] >= -stack.Value)
                {
                    garrison.Army[stack.Key] -= -stack.Value;
                    Army[stack.Key] += -stack.Value;
                }
                else return false;
            }
            return true;
        }


        public override bool Equals(object obj)
        {
            var other = obj as Player;
            if (other == null)
                return false;
            return this.Name == other.Name;
        }

        public override int GetHashCode()
        {
            int hash = 37;
            unchecked
            {
                foreach (var c in Name)
                    hash = hash * 101 + Convert.ToByte(c);
            }
            return hash;
        }

        public override string ToString()
        {
            return "Player " + Name;
        }
    }
}
