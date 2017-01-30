using System.Collections.Generic;
using System.Linq;
using CVARC.V2;
using Pudge.Player;
using Pudge.World;

namespace Pudge.Sensors
{
    public class EventSensor : Sensor<List<EventData>, PudgeRobot>
    {
        public override List<EventData> Measure()
        {
            return
                Actor.LastActivatingTime.Values
                .Where(
                    data => data.Start + data.Duration> Actor.World.Clocks.CurrentTime)
                    .ToList();
        }
    }
}