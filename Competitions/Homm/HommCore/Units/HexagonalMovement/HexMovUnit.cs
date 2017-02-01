using CVARC.V2;
using HoMM.Robot;
using HoMM.World;

namespace HoMM.Units.HexagonalMovement
{
    class HexMovUnit : IUnit
    {
        private IHommRobot actor;

        public HexMovUnit(IHommRobot actor)
        {
            this.actor = actor;
        }

        public UnitResponse ProcessCommand(object command)
        {
            var movement = Compatibility.Check<IHexMovCommand>(this, command).Movement;
            if (movement == null) return UnitResponse.Denied();

            var movementResult = movement.TryMoveHero(
                actor.World.HommEngine, actor.Player, actor.World.Round.Map);

            var newLocation = movementResult.Item1;
            var movementDuration = movementResult.Item2;
            
            actor.World.Clocks.AddTrigger(actor.LocationTrigger = new LocationTrigger(
                actor.World.Clocks.CurrentTime, movementDuration / 2, actor, newLocation));

            return UnitResponse.Accepted(double.PositiveInfinity);
        }
    }
}
