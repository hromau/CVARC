using System;

namespace HoMM.Robot.HexagonalMovement
{
    [Serializable]
    public class HexMovement
    {
        public Direction MovementDirection { get; set; }
        public bool Wait { get; set; }

        public HexMovement(Direction direction)
        {
            MovementDirection = direction;
            Wait = false;
        }

        public HexMovement()
        {
            Wait = true;
        }

        public double Apply(IHommRobot robot)
        {
            return new MovementHelper(robot, MovementDirection).CheckForCombatAndMovePlayer();
        }
    }

}
