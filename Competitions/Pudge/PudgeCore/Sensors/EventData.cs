using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pudge.World;

namespace Pudge.Sensors
{
    public class EventData
    {
        public PudgeEvent Event{ get; set; }
        public double Start{ get; set; }
        public double Duration{ get; set; }
    }
}
