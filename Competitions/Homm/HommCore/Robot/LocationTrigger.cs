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

            Debugger.Settings.EnableType<LocationTrigger>();
            Debugger.Log(actor.Player.Location.X + " " + actor.Player.Location.Y);

            actor.World.HommEngine.SetPosition(actor.ControllerId, actor.Player.Location.X, actor.Player.Location.Y);

            //if (actor.Player.HasNoArmy())
            //    actor.Die();
        }
    }
}
