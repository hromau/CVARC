using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVARC.V2
{
    public class DeathSensor : Sensor<bool, IActor>
    {
        public override bool Measure()
        {
            return Actor.IsDisabled;
        }
    }
}
