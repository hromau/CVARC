using CVARC.V2;
using HoMM.Robot;
using System.Collections.Generic;
using System.Linq;

namespace HoMM.Sensors
{
    public class MapSensor : Sensor<IEnumerable<TileObject>, IHommRobot>
    {
        public override IEnumerable<TileObject> Measure()
        {
            return Actor.World.Round.Map.SelectMany(x => x.Objects);
        }
    }
}
