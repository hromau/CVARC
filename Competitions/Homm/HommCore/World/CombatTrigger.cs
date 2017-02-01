using CVARC.V2;
using HoMM.Robot;
using HoMM.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM.World
{
    class CombatTrigger : TimerTrigger
    {
        public CombatTrigger(HommWorld world)
            : base(_ => DoCheckCombat(world), HommRules.Current.CombatTriggerInterval)
        { }
        
        private static void DoCheckCombat(HommWorld world)
        {
            var robots = world.Actors
                .Where(x => !x.IsDisabled)
                .Where(x => x is IHommRobot)
                .Cast<IHommRobot>()
                .ToList();

            foreach (var robot in robots)
            {
                var timeInCombat = world.CombatResolver.Resolve(robot);
                
                robot.LocationTrigger.ScheduledTime += timeInCombat;
                robot.ControlTrigger.ScheduledTime += timeInCombat;
            }
        }
    }
}
