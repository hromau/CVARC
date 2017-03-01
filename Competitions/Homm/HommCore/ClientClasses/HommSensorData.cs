using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;
using HoMM.Sensors;

namespace HoMM.ClientClasses
{
    public class HommSensorData
    {
        [FromSensor(typeof(LocationSensor))]
        public LocationInfo Location { get; set; }

        [FromSensor(typeof(MapSensor))]
        public MapData Map { get; set; }

        [FromSensor(typeof(ArmySensor))]
        public Dictionary<UnitType, int> MyArmy { get; set; }

        [FromSensor(typeof(TimeSensor))]
        public double WorldCurrentTime { get; set; } 

        [FromSensor(typeof(ActorIdSensor))]
        public string MyRespawnSide { get; set; }

        [FromSensor(typeof(SelfScoresSensor))]
        public int MyScores { get; set; }

        [FromSensor(typeof(TreasurySensor))]
        public Dictionary<Resource,int> MyTreasury { get; set; }

        [FromSensor(typeof(HoMM.Sensors.DeathSensor))]
        public bool IsDead { get; set; }
    }
}
