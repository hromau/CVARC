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
    //class LocationCheckTrigger : OneTimeTrigger
    //{
    //    public LocationCheckTrigger(double submitTime, double movementDuration, IHommRobot actor, Location newLocation)
    //        : base(submitTime + movementDuration,() => DoCheckLocation(submitTime, movementDuration, actor, newLocation))
    //    { }

    //    private static void DoCheckLocation(
    //        double submitTime, double movementDuration, IHommRobot actor, Location newLocation)
    //    {
    //        if (actor.World.Round.Map[newLocation].Objects.Any(x => x is ICombatable))

    //    }
    //}

    class LocationChangeTrigger : OneTimeTrigger
    {
        public LocationChangeTrigger(double submitTime, double movementDuration, IHommRobot actor, Location newLocation)
            : base(submitTime + movementDuration, () => DoUpdateLocation(actor, newLocation))
        { }

        private static void DoUpdateLocation(IHommRobot actor, Location newLocation)
        {
            actor.World.Round.Update(actor.Player, newLocation);
        }
    }
}
