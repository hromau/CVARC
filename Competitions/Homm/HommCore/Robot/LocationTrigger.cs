using CVARC.V2;
using Infrastructure;

namespace HoMM.Robot
{
    class LocationTrigger : OneTimeTrigger
    {
        public LocationTrigger(double submitTime, double movementDuration, HommRobot actor, Location newLocation)
            : base(submitTime + movementDuration, () => DoUpdateLocation(actor, newLocation))
        { }

        private static void DoUpdateLocation(HommRobot actor, Location newLocation)
        {
            actor.World.Round.Update(actor.Player, newLocation);
            actor.World.HommEngine.Freeze(actor.ControllerId);
        }
    }
}
