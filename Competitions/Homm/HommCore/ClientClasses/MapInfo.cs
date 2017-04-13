using System.Collections.Generic;
using System.Linq;

namespace HoMM.ClientClasses
{
    public class MapObjectData
    {
        public LocationInfo Location { get; set; }

        public Terrain Terrain { get; set; }

        public Wall Wall { get; set; }
        public Garrison Garrison { get; set; }
        public NeutralArmy NeutralArmy { get; set; }
        public Mine Mine { get; set; }
        public Dwelling Dwelling { get; set; }
        public ResourcePile ResourcePile { get; set; }
        public Hero Hero { get; set; }

        string ArmyString(Dictionary<UnitType,int> army)
        {
            return army.Select(z => z.Value.ToString() + " " + z.Key.ToString()).Aggregate((a, b) => a + ", " + b);
        }

        public override string ToString()
        {
            string describe = Terrain.ToString();

            if (Wall != null) describe = "Wall";
            if (Garrison != null) describe = "Garrison with " + ArmyString(Garrison.Army);
            if (NeutralArmy != null) describe = "Neutral army with " + ArmyString(NeutralArmy.Army);
            if (Mine != null) describe = "Mine of "+Mine.Resource.ToString();
            if (Dwelling != null) describe = "Dwelling of " + Dwelling.UnitType;
            if (ResourcePile != null) describe = "Resource pile of "+ResourcePile.Amount+" "+ResourcePile.Resource.ToString();
            if (Hero != null) describe = "Hero with " + ArmyString(Hero.Army);

            return describe ?? "Nothing";
        }
    }

    public enum Terrain
    {
        Grass,
        Snow,
        Desert,
        Marsh,
        Road,
    }

    public class Wall
    {

    }

    public class Garrison
    {
        public string Owner { get; set; }
        public Dictionary<UnitType, int> Army { get; set; }

        public Garrison(string owner, Dictionary<UnitType, int> army)
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
