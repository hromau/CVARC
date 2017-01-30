using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;

namespace Pudge.Units.DaggerUnit
{
    public class DaggerUnit : IUnit
    {
        private readonly IDaggerRobot actor;
        public DaggerUnit(IActor actor)
        {
            this.actor = Compatibility.Check<IDaggerRobot>(this, actor);
        }

        public UnitResponse ProcessCommand(object _command)
        {
            if (actor.IsDisabled) return UnitResponse.Denied();
            var command = Compatibility.Check<IDaggerCommand>(this, _command);
            if (command.MakeDagger)
            {
                actor.Dagger(command.DaggerDestination);
                return UnitResponse.Accepted(0.1);
            }
            return UnitResponse.Denied();
        }
    }
}
