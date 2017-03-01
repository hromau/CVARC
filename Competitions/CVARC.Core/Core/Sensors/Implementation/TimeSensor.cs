using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVARC.V2
{

    public class TimeSensor : Sensor<double, IActor>
    {
        public override double Measure()
        {
            return Actor.World.Clocks.CurrentTime;
        }
    }
}
