using HoMM.Rules;
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
            robot.World.HommEngine.Freeze(robot.ControllerId);
            robot.World.HommEngine.SetPosition(robot.ControllerId, robot.Player.Location.X, robot.Player.Location.Y);

            if (Wait == true)
            {
                //robot.World.HommEngine.Freeze(robot.ControllerId);
                //robot.World.HommEngine.SetPosition(robot.ControllerId, robot.Player.Location.X, robot.Player.Location.Y);
                return HommRules.Current.WaitDuration;
            }

            return new MovementHelper(robot, MovementDirection).CheckForCombatAndMovePlayer();
        }
    }

}
