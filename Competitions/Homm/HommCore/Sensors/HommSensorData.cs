using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;
using HoMM.ClientClasses;

namespace HoMM.Sensors
{
    [DataContract]
    [Serializable]
    public class HommSensorData
    {
        [DataMember]
        [FromSensor(typeof(LocationSensor))]
        public LocationInfo Location { get; set; }

        [DataMember]
        [FromSensor(typeof(MapSensor))]
        public List<MapInfo> Map { get; set; }

        [DataMember]
        [FromSensor(typeof(ArmySensor))]
        public Dictionary<UnitType, int> MyArmy { get; set; }

        [DataMember]
        [FromSensor(typeof(DeathSensor))]
        public bool IsDead { get; set; }
    }
}
