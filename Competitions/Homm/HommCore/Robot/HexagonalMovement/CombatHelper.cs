using CVARC.V2;
using HoMM.Engine;
using HoMM.World;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HoMM.Robot.HexagonalMovement
{
    class CombatHelper
    {
        readonly IHommRobot robot;
        readonly HommWorld world;
        readonly Player player;
        readonly IHommEngine hommEngine;
        ICommonEngine commonEngine;

        public CombatHelper(IHommRobot robot)
        {
            this.robot = robot;
            world = robot.World;
            player = robot.Player;
            hommEngine = world.HommEngine;
            commonEngine = world.CommonEngine;
        }

        public void BeginCombat(ICombatable other, Tile robotTile, Tile otherTile, Action onPlayerWin)
        {
            robotTile.BeginCombat();
            otherTile.BeginCombat();

            hommEngine.Freeze(player.Name);
            hommEngine.Freeze(other.UnityId);

            hommEngine.SetRotation(player.Name, robotTile.Location.GetDirectionTo(otherTile.Location).ToUnityAngle());
            hommEngine.SetRotation(other.UnityId, otherTile.Location.GetDirectionTo(robotTile.Location).ToUnityAngle());

            hommEngine.SetAnimation(player.Name, Animation.Attack);
            hommEngine.SetAnimation(other.UnityId, Animation.Attack);

            var combatTriggerTime = world.Clocks.CurrentTime + HommRules.Current.CombatDuration;

            world.Clocks.AddTrigger(new OneTimeTrigger(combatTriggerTime, () =>
            {
                var initialOtherArmy = new Dictionary<UnitType, int>(other.Army);

                Combat.ResolveBattle(player, other);

                var armies = new ArmiesPair(player.Army, other.Army);
                armies.Log("after Combat.ResolveBattle");

                if (other.HasNoArmy())
                {
                    Debugger.Log("other has no army");

                    if (other is TileObject)
                    {
                        Debugger.Log("other is tile object, delete it");

                        ((TileObject)other).OnRemove();
                    }

                    if (other is Player)
                    {
                        var otherPlayer = other as Player;

                        world.Actors
                            .Where(x => x is IHommRobot)
                            .Cast<IHommRobot>()
                            .Where(x => x.Player.Name == otherPlayer.Name)
                            .Single()
                            .Die();
                    }

                    player.OnVictoryAchieved(other, initialOtherArmy);
                }
                else
                {
                    hommEngine.SetAnimation(other.UnityId, Animation.Idle);
                }

                if (player.HasNoArmy())
                {
                    robot.Die();
                }
                else
                {
                    onPlayerWin();
                }

                robotTile.EndCombat();
                otherTile.EndCombat();
            }));
        }
    }
}
