using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;

namespace HoMM.Sensors
{
    [DataContract]
    public class HommSensorData
    {
        [DataMember]
        [FromSensor(typeof(LocationSensor))]
        public Location Location { get; set; }

        [DataMember]
        [FromSensor(typeof(MapSensor))]
        public IEnumerable<TileObject> Map { get; set; }
    }
}
