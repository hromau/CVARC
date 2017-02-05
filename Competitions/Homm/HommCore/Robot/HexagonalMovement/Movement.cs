using Infrastructure;
using HoMM.Engine;
using HoMM.Robot;
using HoMM.Rules;
using System;
using System.Linq;
using CVARC.V2;

namespace HoMM.Units.HexagonalMovement
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
            var player = robot.Player;
            var map = robot.World.Round.Map;
            var engine = robot.World.HommEngine;

            var newLocation = player.Location.NeighborAt(MovementDirection);

            var travelIsPossible = newLocation.IsInside(map.Size) && map[newLocation].IsAvailable;

            if (!travelIsPossible)
            {
                engine.Freeze(player.Name);
                engine.SetPosition(player.Name, player.Location.X, player.Location.Y);

                return HommRules.Current.MovementFailsDuration;
            }

            var combatTarget = map[newLocation].Objects
                .Where(x => x is ICombatable)
                .Cast<ICombatable>()
                .FirstOrDefault();

            if (combatTarget != null)
            {
                BeginCombat(robot, combatTarget, map[robot.Player.Location], map[newLocation]);
                return GetTravelDuration(player, map) + HommRules.Current.CombatDuration;
            }

            var otherRobot = robot.World.Actors
                .Where(x => x is IHommRobot)
                .Cast<IHommRobot>()
                .Where(x => x.Player.Location == newLocation)
                .FirstOrDefault();

            if (otherRobot != null)
            {
                BeginCombat(robot, otherRobot.Player, map[robot.Player.Location], map[newLocation]);
                return GetTravelDuration(player, map) + HommRules.Current.CombatDuration;
            }

            var collisionRobot = robot.World.Actors
                .Where(x => x is IHommRobot)
                .Cast<IHommRobot>()
                .Where(x => x.Player.DisiredLocation == newLocation)
                .FirstOrDefault();

            if (collisionRobot != null)
            {
                robot.World.Clocks.AddTrigger(new OneTimeTrigger(collisionRobot.ControlTrigger.ScheduledTime,
                    () => {
                        BeginCombat(robot, collisionRobot.Player, map[robot.Player.Location], map[newLocation]);
                    }));

                collisionRobot.ControlTrigger.ScheduledTime += HommRules.Current.CombatDuration;

                return double.PositiveInfinity;
            }

            MakeTurn(robot);
            return GetTravelDuration(player, map);
        }

        private void BeginCombat(IHommRobot robot, ICombatable other, Tile robotTile, Tile otherTile)
        {
            robotTile.BeginCombat();
            otherTile.BeginCombat();

            robot.World.HommEngine.Freeze(robot.Player.Name);
            robot.World.HommEngine.Freeze(other.UnityId);

            // TODO: show combat animation

            var combatTriggerTime = robot.World.Clocks.CurrentTime + HommRules.Current.CombatDuration;

            robot.World.Clocks.AddTrigger(new OneTimeTrigger(combatTriggerTime, () =>
            {
                Combat.ResolveBattle(robot.Player, other);

                if (other.HasNoArmy())
                {
                    if (other is TileObject)
                    {
                        robot.World.CommonEngine.DeleteObject(other.UnityId);
                        otherTile.Objects.Remove((TileObject)other);
                    }

                    if (other is Player)
                    {
                        var otherPlayer = other as Player;

                        robot.World.Actors
                            .Where(x => x is IHommRobot)
                            .Cast<IHommRobot>()
                            .Where(x => x.Player.Name == otherPlayer.Name)
                            .Single()
                            .Die();
                    }
                }

                if (robot.Player.HasNoArmy())
                {
                    robot.Die();
                }
                else
                {
                    robot.ControlTrigger.ScheduledTime =
                        robot.World.Clocks.CurrentTime + GetTravelDuration(robot.Player, robot.World.Round.Map) + 0.001;

                    MakeTurn(robot);
                }

                robotTile.EndFight();
                otherTile.EndFight();
            }));
        }

        private void MakeTurn(IHommRobot robot)
        {
            var travelDuration = GetTravelDuration(robot.Player, robot.World.Round.Map);
            var newLocation = robot.Player.Location.NeighborAt(MovementDirection);

            robot.Player.DisiredLocation = newLocation;
            robot.World.HommEngine.Move(robot.Player.Name, MovementDirection, travelDuration);

            robot.World.Clocks.AddTrigger(
                new LocationTrigger(robot.World.Clocks.CurrentTime, travelDuration, robot, newLocation)
            );
        }

        private double GetTravelDuration(Player player, Map map)
        {
            var velocityModifier = map[player.Location].Terrain.TravelCost;
            return HommRules.Current.MovementDuration * velocityModifier;
        }
    }

}
