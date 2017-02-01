using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM
{
    public class Wall : TileObject
    {
        public override bool IsPassable => false;
        public Wall(Location location) : base(location) { }
    }
}
