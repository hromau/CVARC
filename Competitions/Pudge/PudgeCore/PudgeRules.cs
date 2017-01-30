using System;
using System.Collections.Generic;
using AIRLab.Mathematics;
using CVARC.V2;
using Pudge.Player;
using System.Windows.Forms;
using AIRLab;
using Pudge.Units.DaggerUnit;
using Pudge.Units.HookUnit;
using Pudge.Units.PudgeUnit;
using Pudge.Units.WardUnit;
using Pudge.Units.WADUnit;
using Pudge.World;

namespace Pudge
{
    [Serializable]
    public class PudgeRules : IRules, IWADRules, IHookRules, IWardRules, IDaggerRules
    {
        public static readonly PudgeRules Current = new PudgeRules();
        public static string StandingBotName = "Standing";
        private static readonly double keyboardMovementRange = 10;
        private static readonly double keyboardRotationAngle = 5;
        public void DefineKeyboardControl(IKeyboardController _pool, string controllerId)
        {
            var pool = Compatibility.Check<KeyboardController<PudgeCommand>>(this, _pool);

            if (controllerId == TwoPlayersId.Left)
            {
                pool.Add(Keys.W, () => new PudgeCommand { GameMovement = new GameMovement { Range = keyboardMovementRange} });
                pool.Add(Keys.A,
                    () => new PudgeCommand { GameMovement = new GameMovement { Angle = keyboardRotationAngle} });
                pool.Add(Keys.D,
                    () => new PudgeCommand {GameMovement = new GameMovement {Angle = -keyboardRotationAngle}});
                pool.Add(Keys.Space, () => new PudgeCommand { MakeHook = true });
                pool.Add(Keys.F, () => new PudgeCommand {MakeDagger = true,
                    DaggerDestination = new DaggerDestinationPoint(0, 0) });
                pool.Add(Keys.G, () => new PudgeCommand {MakeWard = true});
            }
            else
            {
                pool.Add(Keys.I, () => new PudgeCommand { GameMovement = new GameMovement { Range = MovementRange } });
                pool.Add(Keys.J,
                    () => new PudgeCommand { GameMovement = new GameMovement { Angle = keyboardRotationAngle } });
                pool.Add(Keys.L,
                    () => new PudgeCommand { GameMovement = new GameMovement { Angle = -keyboardRotationAngle } });
                pool.Add(Keys.Enter, () => new PudgeCommand { MakeHook = true });
            }
            pool.StopCommand = () => new PudgeCommand {GameMovement = new GameMovement { WaitTime = WaitDuration}};
        }

        public Bot<PudgeCommand> CreateStandingBot()
        {
            return
                new Bot<PudgeCommand>(
                    turn => new PudgeCommand {GameMovement = new GameMovement { Angle = -keyboardRotationAngle}});
        }


        public int DeathPenalty
        {
            get { return -10; }
        }

        public double HookRange
        {
            get { return 75; }
        }
        
        public double HookAttackRadius
        {
            get { return 12; }
        }
        
        public double HookDuration
        {
            get { return 0.75; }
        }

        public double HookVelocity
        {
            get { return HookRange/HookDuration; }
        }

        public double WaitDuration
        {
            get { return 0.1; }
        }

        public double RotationVelocity
        {
            get { return 360; }
        }

        public double MovementVelocity
        {
            get { return 40; }
        }

        public double RotationAngle
        {
            get { return 5; }
        }

        public double MovementRange
        {
            get { return 10; }
        }



        public double RunePickRange { get { return 15; }}

        public Dictionary<InternalRuneData, int> RuneScores = new Dictionary<InternalRuneData, int>(new InternalRuneData.EqualityComparer())
        {
            {new InternalRuneData(RuneType.GoldXP, RuneSize.Normal), 3},
            {new InternalRuneData(RuneType.GoldXP, RuneSize.Large), 5},
            {new InternalRuneData(RuneType.Haste, RuneSize.Normal), 1},
            {new InternalRuneData(RuneType.Haste, RuneSize.Large), 2},
            {new InternalRuneData(RuneType.Invisibility, RuneSize.Normal), 1},
            {new InternalRuneData(RuneType.Invisibility, RuneSize.Large), 2},
        };

        public int HasteFactor{ get { return 2; } }

        public Dictionary<InternalRuneData, double> BuffDurations = new Dictionary<InternalRuneData, double>(new InternalRuneData.EqualityComparer())
        {
            {new InternalRuneData(RuneType.Haste, RuneSize.Normal), 7},
            {new InternalRuneData(RuneType.Haste, RuneSize.Large), 10},
            {new InternalRuneData(RuneType.Invisibility, RuneSize.Normal), 15},
            {new InternalRuneData(RuneType.Invisibility, RuneSize.Large), 25},
            {new InternalRuneData(RuneType.DoubleDamage, RuneSize.Normal), 15},
            {new InternalRuneData(RuneType.DoubleDamage, RuneSize.Large), 25},
        };
        public double VisibilityRadius{ get { return 61; } }

        public double PudgeRespawnTime { get { return 5; } }

        public double SlardarRespawnTime { get { return 15; } }
        public  double RuneRespawnTime{ get { return 25; } }

        public int PudgeHookScores { get { return 15; } }
        public int SlardarHookScores { get { return 10; } }
        public double HookCooldown{ get { return 8; } }
        public double HookThrown{ get { return double.PositiveInfinity; } }
        public double SlardarWaitDuration{ get { return 3; }}

        public double WardDuration{ get { return 15; } }
        public double WardRadius{ get { return 30; } }
        public double WardIncrementTime{ get { return 15; } }
        public int AvailableWardsAtStart{ get { return 2; } }
        public double DaggerRange{ get { return 90; } }
        public double DaggerCooldown { get { return 15; } }
        public double DoubleDamageFactor{ get { return 1.5; } }
    }
}