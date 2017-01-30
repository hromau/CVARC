using System;

namespace CVARC.V2
{
    public class DeathUnit : IUnit
    {
        readonly Func<double> timeUntilRespawn;

        public DeathUnit(Func<double> timeUntilRespawn)
        {
            this.timeUntilRespawn = timeUntilRespawn;
        }

        public UnitResponse ProcessCommand(object command)
        {
            return UnitResponse.Accepted(Math.Max(timeUntilRespawn() + .01, 0.1));
        }
    }
}
