using CVARC.V2;
using HoMM.Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Sensors
{
    public class ArmySensor : Sensor<Dictionary<UnitType, int>, IHommRobot>
    {
        public override Dictionary<UnitType, int> Measure()
        {
            return new Dictionary<UnitType, int>(Actor.Player.Army);
        }
    }
}
