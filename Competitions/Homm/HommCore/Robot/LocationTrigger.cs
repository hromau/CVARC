using CVARC.V2;
using HoMM.Robot;
using HoMM.Rules;
using HoMM.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            if (actor.Player.HasNoArmy())
                actor.Die();
        }
    }
}
