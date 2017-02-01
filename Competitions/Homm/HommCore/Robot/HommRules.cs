using CVARC.V2;
using HoMM.Robot;
using HoMM.Units.ArmyInterface;
using HoMM.Units.HexagonalMovement;
using System.Windows.Forms;

namespace HoMM.Rules
{
    class HommRules : IRules
    {
        public static readonly HommRules Current = new HommRules();
        public const string StandingBotName = "Standing";

        public void DefineKeyboardControl(IKeyboardController _pool, string controllerId)
        {
            var pool = Compatibility.Check<KeyboardController<HommCommand>>(this, _pool);

            if (controllerId == TwoPlayersId.Left)
            {
                pool.Add(Keys.W, () => new HommCommand { Movement = new Movement(Direction.Up) });
                pool.Add(Keys.S, () => new HommCommand { Movement = new Movement(Direction.Down) });
                pool.Add(Keys.A, () => new HommCommand { Movement = new Movement(Direction.LeftDown) });
                pool.Add(Keys.D, () => new HommCommand { Movement = new Movement(Direction.RightDown) });
                pool.Add(Keys.Q, () => new HommCommand { Movement = new Movement(Direction.LeftUp) });
                pool.Add(Keys.E, () => new HommCommand { Movement = new Movement(Direction.RightUp) });
                pool.Add(Keys.Space, () => new HommCommand { Order = new PurchaseOrder(10) });
            }
            else if (controllerId == TwoPlayersId.Right)
            {
                pool.Add(Keys.I, () => new HommCommand { Movement = new Movement(Direction.Up) });
                pool.Add(Keys.K, () => new HommCommand { Movement = new Movement(Direction.Down) });
                pool.Add(Keys.J, () => new HommCommand { Movement = new Movement(Direction.LeftDown) });
                pool.Add(Keys.L, () => new HommCommand { Movement = new Movement(Direction.RightDown) });
                pool.Add(Keys.U, () => new HommCommand { Movement = new Movement(Direction.LeftUp) });
                pool.Add(Keys.O, () => new HommCommand { Movement = new Movement(Direction.RightUp) });
                pool.Add(Keys.Enter, () => new HommCommand { Order = new PurchaseOrder(10) });
            }

            pool.StopCommand = () => new HommCommand { Movement = new Wait() };
        }

        public double MovementDuration => 10;
        public double WaitDuration => MovementDuration;
        public double BuyDuration => MovementDuration;
        public double CombatDuration => MovementDuration;
        public double DailyTickInterval => 100;
        public double CombatTriggerInterval => 100;
        public double RespawnInterval => 100;
    }
}
