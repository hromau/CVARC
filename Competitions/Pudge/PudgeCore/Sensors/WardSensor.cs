using System.Collections.Generic;
using CVARC.V2;
using Pudge.Player;
using Pudge.Units.WardUnit;

namespace Pudge.Sensors
{
    public class WardSensor : Sensor<List<Ward>, PudgeRobot>
    {
        public override List<Ward> Measure()
        {
            return Actor.Wards.FindAll(w => w.Start + PudgeRules.Current.WardDuration > Actor.World.Clocks.CurrentTime);
        }
    }
}