using CVARC.V2;
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
            Debugger.Log("HexMovUnit activated");

            var movement = Compatibility.Check<IHexMovCommand>(this, command).Movement;
            if (movement == null) return UnitResponse.Denied();

            Debugger.Log("HexMovUnit accepted command");

            var movementDuration = movement.Apply(actor);

            return UnitResponse.Accepted(movementDuration + 0.001);
        }
    }
}
