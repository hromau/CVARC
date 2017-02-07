using CVARC.V2;
using HoMM.Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.Sensors
{
    public class LocationSensor : Sensor<Location, IHommRobot>
    {
        public override Location Measure() => Actor.Player.Location;
    }
}
