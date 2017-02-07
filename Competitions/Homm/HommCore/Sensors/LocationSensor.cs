using CVARC.V2;
using HoMM.ClientClasses;
using HoMM.Robot;

namespace HoMM.Sensors
{
    public class LocationSensor : Sensor<LocationInfo, IHommRobot>
    {
        public override LocationInfo Measure() =>
            new LocationInfo(Actor.Player.Location.X, Actor.Player.Location.Y);
    }
}
