using CVARC.V2;
using HoMM.Robot;

namespace HoMM.Sensors
{
    public class DeathSensor : Sensor<bool, HommRobot>
    {
        public override bool Measure()
        {
            return Actor.IsDead;
        }
    }
}
