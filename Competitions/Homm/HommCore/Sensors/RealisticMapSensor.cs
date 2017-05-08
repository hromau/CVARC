using System.Collections.Generic;
using HoMM.ClientClasses;

namespace HoMM.Sensors
{
    public class RealisticMapSensor : BaseMapSensor<RoughQuantity>
    {
        protected override Dictionary<UnitType, RoughQuantity> ConvertArmy(Dictionary<UnitType, int> internalRepresentation)
        {
            return new RealisticArmy(internalRepresentation);
        }
    }
}
