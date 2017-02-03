using AIRLab;
using AIRLab.Mathematics;
using CVARC.V2;
using Pudge.Player;
using Pudge.Units.WADUnit;
using Pudge.World;
using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;

namespace Pudge
{
    public class DebugLogicPartHelper : TestLoadableLogicPartHelper
    {
        public override LogicPart Initialize(LogicPart logicPart)
        {
            var rules = new PudgeRules();

            logicPart.CreateWorld = () => new PudgeWorld(true);
            logicPart.CreateDefaultSettings = () => new GameSettings { OperationalTimeLimit = 5, TimeLimit = 90 };

            logicPart.WorldStateType = typeof(PudgeWorldState);
            logicPart.CreateWorldState = seed => new PudgeWorldState(int.Parse(seed));
            logicPart.PredefinedWorldStates.AddRange(Enumerable.Range(0, 5).Select(i => i.ToString()));

            var actorFactory = ActorFactory.FromRobot(new PudgeRobot(), rules);
            logicPart.Actors[TwoPlayersId.Left] = actorFactory;
            logicPart.Actors[TwoPlayersId.Right] = actorFactory;

            var slardarFactory = ActorFactory.FromRobot(new Slardar(), new SlardarRules());
            logicPart.NPC.Add(Tuple.Create(SlardarId.LeftTop, slardarFactory, CreateStandingBot()));
            logicPart.NPC.Add(Tuple.Create(SlardarId.RightBottom, slardarFactory, CreateWalkingBot()));

            logicPart.Bots[PudgeRules.StandingBotName] = () => rules.CreateStandingBot();

            return logicPart;
        }

        static Func<IActor, IController> CreateStandingBot()
        {
            return actor => new Bot<SlardarCommand>(turn => new SlardarCommand()
            {
                GameMovement = new GameMovement
                {
                    WaitTime = PudgeRules.Current.WaitDuration
                }
            });
        }

        static Func<IActor, IController> CreateWalkingBot()
        {
            var step = 10 * Math.Sqrt(2);
            var startPoint = new Frame3D(70, 70, 0);

            return actor => new SlardarBot(new List<Frame3D>
            {
                startPoint,
                new Frame3D(startPoint.X, -startPoint.Y, 0),
            }, actor, SlardarId.LeftTop);
        }
    }
}
