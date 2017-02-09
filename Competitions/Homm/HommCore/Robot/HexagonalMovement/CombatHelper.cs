using CVARC.V2;
using HoMM.Engine;
using HoMM.Rules;
using HoMM.World;
using System;
using System.Linq;

namespace HoMM.Robot.HexagonalMovement
{
    class CombatHelper
    {
        IHommRobot robot;
        HommWorld world;
        Player player;
        IHommEngine hommEngine;
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

            // TODO: show combat animation

            var combatTriggerTime = world.Clocks.CurrentTime + HommRules.Current.CombatDuration;

            world.Clocks.AddTrigger(new OneTimeTrigger(combatTriggerTime, () =>
            {
                Combat.ResolveBattle(player, other);

                if (other.HasNoArmy())
                {
                    if (other is TileObject)
                    {
                        commonEngine.DeleteObject(other.UnityId);
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
                }

                if (player.HasNoArmy())
                {
                    robot.Die();
                }
                else
                {
                    onPlayerWin();
                }

                robotTile.EndFight();
                otherTile.EndFight();
            }));
        }
    }
}
