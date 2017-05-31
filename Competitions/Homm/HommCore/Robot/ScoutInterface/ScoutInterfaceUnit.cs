using CVARC.V2;
using System;

namespace HoMM.Robot.ScoutInterface
{
    class ScoutInterfaceUnit : IUnit
    {
        private IHommRobot actor;

        public ScoutInterfaceUnit(IHommRobot actor)
        {
            this.actor = actor;
        }

        public UnitResponse ProcessCommand(object command)
        {
            var scoutOrder = Compatibility.Check<IScoutCommand>(this, command).ScoutOrder;

            if (scoutOrder == null)
                return UnitResponse.Denied();

            if (!scoutOrder.ScoutHero ^ scoutOrder.ScoutTile)
                throw new ArgumentException("Please scout either hero or tile at one time.");

            if (!actor.Player.Scout.IsAvailable())
                throw new InvalidOperationException("Scouting is not available at a time. " +
                    "Please wait `HommRules.ScoutingCooldown` before sending a new scout.");

            return UnitResponse.Accepted(actor.Player.Scout.Execute(scoutOrder));
        }
    }
}
