using HoMM.ClientClasses;
using HoMM.Engine;
using System;

namespace HoMM.Robot.HexagonalMovement
{
    [Serializable]
    public class HexMovement
    {
        public Direction MovementDirection { get; set; }
        public double WaitDuration { get; set; }

        public HexMovement() { }

        public HexMovement(Direction direction)
        {
            MovementDirection = direction;
            WaitDuration = 0;
        }

        public HexMovement(double waitDuration)
        {
            WaitDuration = waitDuration;
        }

        internal double Apply(IHommRobot robot)
        {
            robot.World.HommEngine.Freeze(robot.ControllerId);
            robot.World.HommEngine.SetPosition(robot.ControllerId, robot.Player.Location.X, robot.Player.Location.Y);

            if (WaitDuration != 0)
            {
                robot.World.HommEngine.SetAnimation(robot.ControllerId, Animation.Idle);
                return WaitDuration;
            }

            return new MovementHelper(robot, MovementDirection).CheckForCombatAndMovePlayer();
        }
    }

}
