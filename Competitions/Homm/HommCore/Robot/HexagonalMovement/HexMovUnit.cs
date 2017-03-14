using CVARC.V2;
using HoMM.Robot;
using HoMM.World;
using Infrastructure;

namespace HoMM.Robot.HexagonalMovement
{
    class HexMovUnit : IUnit
    {
        private HommRobot actor;

        public HexMovUnit(HommRobot actor)
        {
            this.actor = actor;
        }

        public UnitResponse ProcessCommand(object command)
        {
            Debugger.Log("Enter process command");

            var movement = Compatibility.Check<IHexMovCommand>(this, command).Movement;
            if (movement == null) return UnitResponse.Denied();
            Debugger.Log("Accepted HexMovCommand");


            var movementDuration = movement.Apply(actor);

            return UnitResponse.Accepted(movementDuration + 0.001);
        }
    }
}
