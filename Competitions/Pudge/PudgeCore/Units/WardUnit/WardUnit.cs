using CVARC.V2;
using Pudge.Player;
using Pudge.Units.WADUnit;

namespace Pudge.Units.WardUnit
{
    public class WardUnit : IUnit
    {
        private readonly IWardRobot actor;
        public WardUnit(IActor actor)
        {
            this.actor = Compatibility.Check<IWardRobot>(this, actor);
        }
        public UnitResponse ProcessCommand(object _command)
        {
            if (actor.IsDisabled) return UnitResponse.Denied();
            var command = Compatibility.Check<IWardCommand>(this, _command);
            if (command.MakeWard)
            {
                actor.SpawnWard();
                return UnitResponse.Accepted(0.1);
            }
            return UnitResponse.Denied();
        }
    }
}