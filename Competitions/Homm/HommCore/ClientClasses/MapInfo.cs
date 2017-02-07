using System.Collections.Generic;

namespace HoMM.ClientClasses
{
    public class MapInfo
    {
        public LocationInfo Location { get; set; }

        public Wall Wall { get; set; }
        public Garrison Garrison { get; set; }
        public NeutralArmy NeutralArmy { get; set; }
        public Mine Mine { get; set; }
        public Dwelling Dwelling { get; set; }
        public ResourcePile ResourcePile { get; set; }
        public Hero Hero { get; set; }
    }

    public class Wall
    {

    }

    public class Garrison
    {
        public Hero Owner { get; set; }
        public Dictionary<UnitType, int> Army { get; set; }

        public Garrison(Hero owner, Dictionary<UnitType, int> army)
        {
            Owner = owner;
            Army = new Dictionary<UnitType, int>(army);
        }
    }

    public class NeutralArmy
    {
        public Dictionary<UnitType, int> Army { get; set; }

        public NeutralArmy(Dictionary<UnitType, int> army)
        {
            Army = new Dictionary<UnitType, int>(army);
        }
    }

    public class Mine
    {
        public Resource Resource { get; set; }
        public Hero Owner { get; set; }

        public Mine(Resource resource, Hero owner)
        {
            Resource = resource;
            Owner = owner;
        }
    }

    public class Dwelling
    {
        public UnitType UnitType { get; set; }
        public int AvailableToBuyCount { get; set; }

        public Dwelling(UnitType unitType, int availableCount)
        {
            UnitType = unitType;
            AvailableToBuyCount = availableCount;
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

    public class Hero
    {
        public string Name { get; set; }
        public Dictionary<UnitType, int> Army { get; set; }

        public Hero(string name, Dictionary<UnitType, int> army)
        {
            Name = name;
            Army = new Dictionary<UnitType, int>(army);
        }
    }
}
