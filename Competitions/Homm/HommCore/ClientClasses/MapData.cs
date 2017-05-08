using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.ClientClasses
{
    public class MapData<TArmy>
    {
        public List<MapObjectData<TArmy>> Objects { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

    }
}
