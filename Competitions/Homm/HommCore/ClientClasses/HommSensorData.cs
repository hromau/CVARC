using System.Collections.Generic;
using CVARC.V2;
using HoMM.Sensors;

namespace HoMM.ClientClasses
{
    public abstract class BaseHommSensorData
    {
        [FromSensor(typeof(LocationSensor))]
        public LocationInfo Location { get; set; }

        [FromSensor(typeof(ArmySensor))]
        public Dictionary<UnitType, int> MyArmy { get; set; }

        [FromSensor(typeof(TimeSensor))]
        public double WorldCurrentTime { get; set; }

        [FromSensor(typeof(ActorIdSensor))]
        public string MyRespawnSide { get; set; }

        [FromSensor(typeof(TreasurySensor))]
        public Dictionary<Resource, int> MyTreasury { get; set; }

        [FromSensor(typeof(HoMM.Sensors.DeathSensor))]
        public bool IsDead { get; set; }
    }

    public class HommSensorData : BaseHommSensorData
    {
        [FromSensor(typeof(MapSensor))]
        public MapData<int> Map { get; set; }
    }

    public class HommFinalSensorData : BaseHommSensorData
    {

        [FromSensor(typeof(RealisticMapSensor))]
        public MapData<RoughQuantity> Map { get; set; }
    }
}
