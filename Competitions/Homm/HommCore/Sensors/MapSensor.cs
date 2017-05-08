using System.Collections.Generic;

namespace HoMM.Sensors
{
    public class MapSensor : BaseMapSensor<int>
    {
        protected override Dictionary<UnitType, int> ConvertArmy(Dictionary<UnitType, int> internalRepresentation)
        {
            return new Dictionary<UnitType, int>(internalRepresentation);
        }
    }
}
