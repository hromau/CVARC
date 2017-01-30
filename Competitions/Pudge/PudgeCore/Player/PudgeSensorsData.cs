using System.Collections.Generic;
using System.Runtime.Serialization;
using CVARC.V2;
using Pudge.GameExceptions;
using Pudge.Sensors;
using Pudge.Sensors.Map;
using Pudge.Units.WardUnit;
using Pudge.Utils;

namespace Pudge.Player
{
    [DataContract]
    public class PudgeSensorsData
    {

        [DataMember]
        [FromSensor(typeof (SelfLocationSensor))]
        public LocatorItem SelfLocation{ get; set; }

        [DataMember]
        [FromSensor(typeof (SelfScoreSensor))]
        public int SelfScores{ get; set; }

        [DataMember]
        [FromSensor(typeof (MapSensor))]
        public Map Map{ get; set; }

        [DataMember]
        [FromSensor(typeof(EventSensor))]
        public List<EventData> Events{ get; set;} 

        [DataMember]
        [FromSensor(typeof(DeathSensor))]
        public bool IsDead { get; set; }

        [DataMember]
        [FromSensor(typeof(TimeSensor))]
        public double WorldTime{ get; set; }

        [DataMember]
        [FromSensor(typeof(WardSensor))]
        public List<Ward> Wards { get; set; }

    }
}