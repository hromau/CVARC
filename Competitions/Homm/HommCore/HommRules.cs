using CVARC.V2;
using HoMM.Robot;
using HoMM.Robot.ArmyInterface;
using HoMM.Robot.HexagonalMovement;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HoMM.ClientClasses
{
    public class HommRules : IRules
    {
      
        public static readonly HommRules Current = new HommRules();
        public const string StandingBotName = "Standing";

        public void DefineKeyboardControl(IKeyboardController _pool, string controllerId)
        {
            var pool = Compatibility.Check<KeyboardController<HommCommand>>(this, _pool);

            if (controllerId == TwoPlayersId.Left)
            {
                pool.Add(Keys.W, () => new HommCommand { Movement = new HexMovement(Direction.Up) });
                pool.Add(Keys.S, () => new HommCommand { Movement = new HexMovement(Direction.Down) });
                pool.Add(Keys.A, () => new HommCommand { Movement = new HexMovement(Direction.LeftDown) });
                pool.Add(Keys.D, () => new HommCommand { Movement = new HexMovement(Direction.RightDown) });
                pool.Add(Keys.Q, () => new HommCommand { Movement = new HexMovement(Direction.LeftUp) });
                pool.Add(Keys.E, () => new HommCommand { Movement = new HexMovement(Direction.RightUp) });

                pool.Add(Keys.Space, () => new HommCommand { Order = new HireOrder(5) });

                pool.Add(Keys.X, () => new HommCommand
                {
                    WaitInGarrison = new Dictionary<UnitType, int> { { UnitType.Militia, 5 } }
                });
            }
            else if (controllerId == TwoPlayersId.Right)
            {
                pool.Add(Keys.I, () => new HommCommand { Movement = new HexMovement(Direction.Up) });
                pool.Add(Keys.K, () => new HommCommand { Movement = new HexMovement(Direction.Down) });
                pool.Add(Keys.J, () => new HommCommand { Movement = new HexMovement(Direction.LeftDown) });
                pool.Add(Keys.L, () => new HommCommand { Movement = new HexMovement(Direction.RightDown) });
                pool.Add(Keys.U, () => new HommCommand { Movement = new HexMovement(Direction.LeftUp) });
                pool.Add(Keys.O, () => new HommCommand { Movement = new HexMovement(Direction.RightUp) });

                pool.Add(Keys.Enter, () => new HommCommand { Order = new HireOrder(5) });

                pool.Add(Keys.M, () => new HommCommand
                {
                    WaitInGarrison = new Dictionary<UnitType, int> { { UnitType.Militia, 5 } }
                });
            }

            pool.StopCommand = () => new HommCommand { Movement = new HexMovement(waitDuration: 0.1) };
        }

        public double MovementDuration => 0.5;
        public double MovementFailsDuration => 0.1;
        public double UnitsHireDuration => MovementDuration;
        public double GarrisonBuildDuration => MovementDuration;
        public double CombatDuration => 2;
        public double DailyTickInterval => 5;
        public double RespawnInterval => 2;

        public int MineOwningDailyScores => 1;
        public int ResourcesGainScores => 1;

        public int MineDailyResourceYield => 10;
        public int DwellingCapacity => 32;

        public double HeroViewRadius => 5;

        internal UnitsConstants Units { get; } = new UnitsConstants();
    }
}
