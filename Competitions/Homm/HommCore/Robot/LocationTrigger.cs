using CVARC.V2;
using Infrastructure;

namespace HoMM.Robot
{
    class LocationTrigger : OneTimeTrigger
    {
        public LocationTrigger(double submitTime, double movementDuration, IHommRobot actor, Location newLocation)
            : base(submitTime + movementDuration, () => DoUpdateLocation(actor, newLocation))
        { }

        private static void DoUpdateLocation(IHommRobot actor, Location newLocation)
        {
            actor.World.Round.Update(actor.Player, newLocation);
            actor.World.HommEngine.Freeze(actor.ControllerId);
        }
    }
}
