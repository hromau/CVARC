using CVARC.V2;
using HoMM.ClientClasses;
using HoMM.Engine;
using HoMM.Robot;
using HoMM.Robot.ArmyInterface;
using HoMM.Robot.HexagonalMovement;
using HoMM.World;
using Infrastructure;
using System;
using System.Linq;

namespace HoMM
{
    public class HommLogicPartHelper : LogicPartHelper
    {
        int playersCount;
        bool limitView;

        static string[] pids = new string[]
        {
            TwoPlayersId.Left,
            TwoPlayersId.Right,
        };

        public HommLogicPartHelper(int playersCount, bool limitView)
        {
            if (playersCount <= 0 && playersCount > pids.Length)
                throw new ArgumentOutOfRangeException(
                    $"{playersCount} player(s) mode is not supported! Try 1 or 2.");

            this.playersCount = playersCount;
            this.limitView = limitView;
        }

        public override LogicPart Create()
        {
            var logicPart = new LogicPart();
            var rules = new HommRules();

            logicPart.CreateWorld = () => new HommWorld(pids.Take(playersCount).ToArray());
            logicPart.CreateDefaultSettings = () => new GameSettings
            {
                OperationalTimeLimit = 5,
                TimeLimit = 90,
                SpectacularView = true,
            };

            logicPart.WorldStateType = typeof(HommWorldState);
            logicPart.CreateWorldState = seed => seed == "debug" ? new HommWorldState(0, true) : new HommWorldState(Int32.Parse(seed), false);

            logicPart.PredefinedWorldStates.Add("debug");

            var actorFactory = ActorFactory.FromFunction(() => new HommRobot(limitView ? rules.HeroViewRadius : Double.PositiveInfinity), rules);
            
            foreach (var pid in pids.Take(playersCount))
                logicPart.Actors[pid] = actorFactory;

            logicPart.Bots[HommRules.StandingBotName] = () => 
                new Bot<HommCommand>(_ => new HommCommand { Movement = new HexMovement(waitDuration: 0.5) });

            return logicPart;
        }
    }
}
