using System;

namespace HoMM.Robot.HexagonalMovement
{
    [Serializable]
    class Movement : IMovement
    {
        public Direction MovementDirection { get; }

        public Movement(Direction direction)
        {
            MovementDirection = direction;
        }

        public double Apply(IHommRobot robot)
        {
            return new MovementHelper(robot, MovementDirection).CheckForCombatAndMovePlayer();
        }
    }

}
