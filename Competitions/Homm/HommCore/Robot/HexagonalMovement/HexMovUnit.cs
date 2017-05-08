using CVARC.V2;
using HoMM.ClientClasses;
using HoMM.Robot;
using HoMM.World;
using Infrastructure;

namespace HoMM.Robot.HexagonalMovement
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
            Debugger.Log("Enter process command");

            var movement = Compatibility.Check<IHexMovCommand>(this, command).Movement;
            if (movement == null) return UnitResponse.Denied();
            Debugger.Log("Accepted HexMovCommand");

            if (actor.IsDead)
                return UnitResponse.Accepted(movement.WaitDuration > 0 
                    ? movement.WaitDuration 
                    : HommRules.Current.MovementDuration);

            var movementDuration = movement.Apply(actor);

            return UnitResponse.Accepted(movementDuration + 0.001);
        }
    }
}
