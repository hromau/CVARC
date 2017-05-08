using System.Collections.Generic;

namespace HoMM.ClientClasses
{
    public class Wall
    {

    }

    public class Garrison<TUnitsCount>
    {
        public string Owner { get; set; }
        public IDictionary<UnitType, TUnitsCount> Army { get; set; }

        public Garrison(string owner, IDictionary<UnitType, TUnitsCount> army)
        {
            Owner = owner;
            Army = army;
        }
    }

    public class NeutralArmy<TUnitsCount>
    {
        public IDictionary<UnitType, TUnitsCount> Army { get; set; }

        public NeutralArmy(IDictionary<UnitType, TUnitsCount> army)
        {
            Army = army;
        }
    }

    public class Mine
    {
        public Resource Resource { get; set; }
        public string Owner { get; set; }

        public Mine(Resource resource, string owner)
        {
            Resource = resource;
            Owner = owner;
        }
    }

    public class Dwelling
    {
        public UnitType UnitType { get; set; }
        public int AvailableToBuyCount { get; set; }
        public string Owner { get; set; }

        public Dwelling(UnitType unitType, int availableCount, string owner)
        {
            UnitType = unitType;
            AvailableToBuyCount = availableCount;
            Owner = owner;
        }
    }

    public class ResourcePile
    {
        public Resource Resource { get; set; }
        public int Amount { get; set; }

        public ResourcePile(Resource resource, int amount)
        {
            Resource = resource;
            Amount = amount;
        }
    }

    public class Hero<TUnitsCount>
    {
        public string Name { get; set; }
        public IDictionary<UnitType, TUnitsCount> Army { get; set; }

        public Hero(string name, IDictionary<UnitType, TUnitsCount> army)
        {
            Name = name;
            Army = army;
        }
    }
}
