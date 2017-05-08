using System.Collections.Generic;
using System.Linq;

namespace HoMM.ClientClasses
{
    public class MapObjectData<TUnitType>
    {
        public LocationInfo Location { get; set; }
        public Terrain Terrain { get; set; }

        public Wall Wall { get; set; }
        public Mine Mine { get; set; }
        public Dwelling Dwelling { get; set; }
        public ResourcePile ResourcePile { get; set; }

        public Garrison<TUnitType> Garrison { get; set; }
        public NeutralArmy<TUnitType> NeutralArmy { get; set; }
        public Hero<TUnitType> Hero { get; set; }

        private static string ArmyString(IDictionary<UnitType, TUnitType> army)
        {
            return army.Select(z => z.Value.ToString() + " " + z.Key.ToString()).Aggregate((a, b) => a + ", " + b);
        }

        public override string ToString()
        {
            var describe = Terrain.ToString();

            if (Wall != null) describe = "Wall";
            if (Mine != null) describe = "Mine of " + Mine.Resource;
            if (Dwelling != null) describe = "Dwelling of " + Dwelling.UnitType;
            if (ResourcePile != null) describe = "Resource pile of " + ResourcePile.Amount + " " + ResourcePile.Resource;
            if (Garrison != null) describe = "Garrison with " + ArmyString(Garrison.Army);
            if (NeutralArmy != null) describe = "Neutral army with " + ArmyString(NeutralArmy.Army);
            if (Hero != null) describe = "Hero with " + ArmyString(Hero.Army);

            return describe;
        }
    }
}