using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HoMM.ClientClasses;

namespace HoMM.Robot.ScoutInterface
{
    public class ScoutOrder
    {
        public bool ScoutHero { get; set; }
        public bool ScoutTile { get; set; }
        public LocationInfo TileToScout { get; set; }

        public static ScoutOrder ScoutHeroOrder()
        {
            return new ScoutOrder { ScoutHero = true };
        }

        public static ScoutOrder ScoutTileOrder(LocationInfo toScout)
        {
            return new ScoutOrder { ScoutTile = true, TileToScout = toScout };
        }
    }
}
