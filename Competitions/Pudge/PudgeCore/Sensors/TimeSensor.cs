using System;
using CVARC.V2;
using Pudge.Player;

namespace Pudge.Sensors
{
    public class TimeSensor : Sensor<double, PudgeRobot>
    {
        public override double Measure()
        {
            return Actor.World.Clocks.CurrentTime;
        }
    }
}