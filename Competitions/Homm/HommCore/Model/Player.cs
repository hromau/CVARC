using CVARC.V2;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoMM
{
    public class Player : ICombatable
    {
        public string Name { get; private set; }
        private Map map;
        public Dictionary<Resource, int> Resources { get; }
        public Location Location { get; set; }
        public Location DisiredLocation { get; set; }
        public Dictionary<UnitType, int> Army { get; }

        public string UnityId => Name;

        public Player(string name, Map map)
        {
            Name = name;
            Resources = new Dictionary<Resource, int>();
            foreach (Resource res in Enum.GetValues(typeof(Resource)))
                Resources.Add(res, 0);
            Army = new Dictionary<UnitType, int>();
            foreach (UnitType t in Enum.GetValues(typeof(UnitType)))
                Army.Add(t, 0);
            this.map = map;
        }


        public event Action<Resource, int> ResourcesGained;
        public event Action<Resource, int> ResourcesUpdated;
        public event Action<ICombatable, Dictionary<UnitType, int>> VictoryAchieved;
        public event Action<UnitType, int> ArmyUpdated;
        public event Action<UnitType, int> UnitsAdded;
        public event Action<Mine> OwnMineForHour;

        internal void OnResourcesGained(Resource resource, int deltaAmount) => ResourcesGained?.Invoke(resource, deltaAmount);
        internal void OnResourcesUpdated(Resource resource, int currentAmount) => ResourcesUpdated?.Invoke(resource, currentAmount);
        internal void OnVictoryAchieved(ICombatable opponent, Dictionary<UnitType, int> army) => VictoryAchieved?.Invoke(opponent, army);
        internal void OnArmyUpdated(UnitType unit, int count) => ArmyUpdated?.Invoke(unit, count);
        internal void OnUnitsAdded(UnitType unit, int count) => UnitsAdded?.Invoke(unit, count);
        internal void OnMineOwnForHour(Mine mine) => OwnMineForHour?.Invoke(mine);

        public int CheckResourceAmount(Resource res)
        {
            return Resources[res];
        }
        public Dictionary<Resource, int> CheckAllResources()
        {
            return new Dictionary<Resource, int>(Resources);
        }

        public void GainResources(Resource res, int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Cannot 'gain' negative resources!");
            Resources[res] += amount;

            OnResourcesGained(res, amount);
            OnResourcesUpdated(res, Resources[res]);

            Debugger.Log($"{Name} got {amount} pieces of {res}");
        }

        public void PayResources(Resource res, int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Cannot 'pay' positive resources!");
            if (amount > Resources[res])
                throw new ArgumentException($"Not enough {res} to pay {amount}");
            Resources[res] -= amount;

            OnResourcesUpdated(res, Resources[res]);
        }


        public void AddUnits(UnitType unitType, int amount)
        {
            if (!Army.ContainsKey(unitType))
                Army.Add(unitType, 0);
            Army[unitType] += amount;
            OnArmyUpdated(unitType, Army[unitType]);
            OnUnitsAdded(unitType, amount);
        }

        public void SetUnitsCount(UnitType unitType, int amount)
        {
            Army[unitType] = amount;
            OnArmyUpdated(unitType, amount);
        }

        public bool TryBuyUnits(int unitsToBuy)
        {
            if (unitsToBuy <= 0)
                throw new ArgumentException("Buy positive amounts of units!");

            var dwelling = map[Location].Objects
                .Where(x => x is Dwelling)
                .Cast<Dwelling>()
                .FirstOrDefault();

            if (dwelling == null)
                return false;

            Debugger.Log($"{dwelling.UnityId}, available units: {dwelling.AvailableUnits} {dwelling.Recruit.UnitType}");

            if (dwelling.Owner != this || dwelling.AvailableUnits < unitsToBuy)
                return false;

            foreach (var kvp in dwelling.Recruit.UnitCost)
                if (CheckResourceAmount(kvp.Key) < kvp.Value * unitsToBuy)
                    return false;
            foreach (var kvp in dwelling.Recruit.UnitCost)
                PayResources(kvp.Key, kvp.Value * unitsToBuy);

            dwelling.RemoveBoughtUnits(unitsToBuy);
            AddUnits(dwelling.Recruit.UnitType, unitsToBuy);

            Debugger.Log($"Purchase: {unitsToBuy} {dwelling.Recruit.UnitType}");

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
