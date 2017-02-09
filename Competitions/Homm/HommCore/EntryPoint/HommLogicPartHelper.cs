using CVARC.V2;
using HoMM.Engine;
using HoMM.Robot;
using HoMM.Rules;
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

        static string[] pids = new string[]
        {
            TwoPlayersId.Left,
            TwoPlayersId.Right,
        };

        public HommLogicPartHelper(int playersCount)
        {
            if (playersCount <= 0 && playersCount > pids.Length)
                throw new ArgumentOutOfRangeException(
                    $"{playersCount} player(s) mode is not supported! Try 1 or 2.");

            this.playersCount = playersCount;
        }

        public override LogicPart Create()
        {
            var logicPart = new LogicPart();
            var rules = new HommRules();

            logicPart.CreateWorld = () => new HommWorld(pids.Take(playersCount).ToArray());
            logicPart.CreateDefaultSettings = () => new GameSettings { OperationalTimeLimit = 5, TimeLimit = 1000 };

            logicPart.WorldStateType = typeof(HommWorldState);
            logicPart.CreateWorldState = seed => new HommWorldState(int.Parse(seed));

            //logicPart.PredefinedWorldStates.AddRange(Enumerable.Range(0, 5).Select(i => i.ToString()));
            logicPart.PredefinedWorldStates.Add(HommRules.DebugSeed.ToString());

            var actorFactory = ActorFactory.FromRobot(new HommRobot(), rules);
            
            foreach (var pid in pids.Take(playersCount))
                logicPart.Actors[pid] = actorFactory;

            logicPart.Bots[HommRules.StandingBotName] = () => 
                new Bot<HommCommand>(_ => new HommCommand { Movement = new HexMovement() });

            return logicPart;
        }
    }
}
