using CVARC.V2;
using HoMM.Engine;
using HoMM.World;
using System.Linq;

namespace HoMM.Robot.HexagonalMovement
{
    class MovementHelper
    {
        readonly Location newLocation;
        readonly IHommEngine hommEngine;
        ICommonEngine commonEngine;
        readonly HommWorld world;
        readonly Player player;
        readonly Map map;
        readonly IHommRobot robot;
        readonly double movementDuration;
        readonly Direction movementDirection;

        public MovementHelper(IHommRobot robot, Direction movementDirection)
        {
            this.robot = robot;
            this.movementDirection = movementDirection;

            world = robot.World;
            hommEngine = world.HommEngine;
            commonEngine = world.CommonEngine;
            player = robot.Player;
            map = world.Round.Map;

            newLocation = player.Location.NeighborAt(movementDirection);
            movementDuration = GetTravelDuration(player, map);
        }

        private double GetTravelDuration(Player player, Map map)
        {
            var velocityModifier = map[player.Location].Terrain.TravelCost;
            return HommRules.Current.MovementDuration * velocityModifier;
        }

        public double CheckForCombatAndMovePlayer()
        {
            if (!CheckNewLocationIsAvailable())
                return HandleNewLocationIsUnavailable();

            var combatTarget = CheckCombatTarget();
            if (combatTarget != null)
                return movementDuration + HandleCombatTarget(combatTarget);

            var otherRobot = CheckOtherRobot();
            if (otherRobot != null)
                return movementDuration + HandleOtherRobot(otherRobot);

            var collisionRobot = CheckMovementCollision();
            if (collisionRobot != null)
                return movementDuration + HandleMovementCollision(collisionRobot);

            MakeTurn(robot);
            return movementDuration;
        }

        private bool CheckNewLocationIsAvailable()
        {
            return newLocation.IsInside(map.Size) && map[newLocation].IsAvailable;
        }

        private double HandleNewLocationIsUnavailable()
        {
            hommEngine.Freeze(player.Name);
            hommEngine.SetPosition(player.Name, player.Location.X, player.Location.Y);
            hommEngine.SetRotation(player.Name, movementDirection.ToUnityAngle());
            hommEngine.SetAnimation(robot.ControllerId, Animation.Gallop);

            return HommRules.Current.MovementFailsDuration;
        }

        private ICombatable CheckCombatTarget()
        {
            return map[newLocation].Objects
                .Where(x => x is ICombatable)
                .Where(x => !(x is CapturableObject) || ((CapturableObject)x).Owner != robot.Player)
                .Cast<ICombatable>()
                .FirstOrDefault();
        }

        private double HandleCombatTarget(ICombatable target)
        {
            BeginCombat(target, map[robot.Player.Location], map[newLocation]);
            return HommRules.Current.CombatDuration;
        }

        private IHommRobot CheckOtherRobot()
        {
            return world.Actors
                .Where(x => x is IHommRobot)
                .Cast<IHommRobot>()
                .Where(x => !x.IsDead)
                .Where(x => x.Player.Location == newLocation)
                .FirstOrDefault();
        }

        private double HandleOtherRobot(IHommRobot otherRobot)
        {
            BeginCombat(otherRobot.Player, map[robot.Player.Location], map[newLocation]);
            otherRobot.ControlTrigger.ScheduledTime += HommRules.Current.CombatDuration;
            return HommRules.Current.CombatDuration;
        }

        private IHommRobot CheckMovementCollision()
        {
            return world.Actors
                .Where(x => x is IHommRobot)
                .Cast<IHommRobot>()
                .Where(x => !x.IsDead)
                .Where(x => x.Player.DisiredLocation == newLocation)
                .FirstOrDefault();
        }

        private double HandleMovementCollision(IHommRobot collisionRobot)
        {
            robot.World.Clocks.AddTrigger(new OneTimeTrigger(collisionRobot.ControlTrigger.ScheduledTime,
                () => BeginCombat(collisionRobot.Player, map[robot.Player.Location], map[newLocation])));

            collisionRobot.ControlTrigger.ScheduledTime += HommRules.Current.CombatDuration;

            return double.PositiveInfinity;
        }

        private void BeginCombat(ICombatable other, Tile robotTile, Tile otherTile)
        {
            new CombatHelper(robot).BeginCombat(other, robotTile, otherTile, () =>
            {
                robot.ControlTrigger.ScheduledTime = world.Clocks.CurrentTime + movementDuration + 0.001;
                MakeTurn(robot);
            });
        }

        private void MakeTurn(IHommRobot robot)
        {
            player.DisiredLocation = newLocation;

            hommEngine.SetAnimation(robot.ControllerId, Animation.Gallop);
            hommEngine.SetRotation(robot.ControllerId, movementDirection.ToUnityAngle());
            hommEngine.Move(robot.Player.Name, movementDirection, movementDuration);

            world.Clocks.AddTrigger(new LocationTrigger(robot.World.Clocks.CurrentTime,
                movementDuration, robot, newLocation));
        }
    }
}
