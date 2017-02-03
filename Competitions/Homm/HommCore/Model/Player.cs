using System;
using System.Collections.Generic;

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
        public Dictionary<UnitType, int> Army { get; }



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
            if (!(map[Location].tileObject is Dwelling))
                return false;

            var d = (Dwelling)map[Location].tileObject;
            if (d.Owner != this)
                return false;
            if (d.AvailableUnits < unitsToBuy)
                return false;
            foreach (var kvp in d.Recruit.UnitCost)
                if (CheckResourceAmount(kvp.Key) < kvp.Value * unitsToBuy)
                    return false;
            foreach (var kvp in d.Recruit.UnitCost)
                PayResources(kvp.Key, kvp.Value * unitsToBuy);
            AddUnits(d.Recruit.UnitType, unitsToBuy);
            return true;
        }

        //exchange units, positive amounts give to garrison, negative take from garrison
        public bool TryExchangeUnitsWithGarrison(Dictionary<UnitType, int> unitsToExchange)
        {
            if (!(map[Location].tileObject is Garrison))
                return false;
            var g = (Garrison)map[Location].tileObject;
            if (g.Owner != this)
                return false;

            foreach (var stack in unitsToExchange)
            {
                if (stack.Value >= 0 && Army[stack.Key] >= stack.Value) 
                {
                    if (!g.Army.ContainsKey(stack.Key))
                        g.Army.Add(stack.Key, 0);
                    g.Army[stack.Key] += stack.Value;
                    Army[stack.Key] -= stack.Value;
                }
                else if (stack.Value < 0 && g.Army.ContainsKey(stack.Key) && g.Army[stack.Key] >= -stack.Value)
                {
                    g.Army[stack.Key] -= -stack.Value;
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
