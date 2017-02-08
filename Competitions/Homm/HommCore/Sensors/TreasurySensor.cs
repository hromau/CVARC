using CVARC.V2;
using HoMM.Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Sensors
{
    public class TreasurySensor : Sensor<Dictionary<Resource, int>, IHommRobot>
    {
        public override Dictionary<Resource, int> Measure()
        {
            return Actor.Player.CheckAllResources();
        }
    }
}
