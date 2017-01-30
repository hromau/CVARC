using System;
using System.Collections.Generic;
using System.Linq;
using AIRLab;
using AIRLab.Mathematics;
using CVARC.V2;
using Pudge.Player;
using Pudge.Units.WADUnit;
using Pudge.World;

namespace Pudge
{
    public class ReleaseLogicPartHelper : TestLoadableLogicPartHelper
    {
        int playersCount;

        public ReleaseLogicPartHelper(int playersCount)
        {
            this.playersCount = playersCount;
        }

        public override LogicPart Initialize(LogicPart logicPart)
        {
            var rules = new PudgeRules();

            logicPart.CreateWorld = () => new PudgeWorld();
            logicPart.CreateDefaultSettings = () => new Settings { OperationalTimeLimit = 5, TimeLimit = 90 };

            logicPart.WorldStateType = typeof(PudgeWorldState);
            logicPart.CreateWorldState = seed => new PudgeWorldState(int.Parse(seed));
            logicPart.PredefinedWorldStates.AddRange(Enumerable.Range(0, 5).Select(i => i.ToString()));

            var actorFactory = ActorFactory.FromRobot(new Player.PudgeRobot(), rules);
            logicPart.Actors[TwoPlayersId.Left] = actorFactory;

            if (playersCount == 2)
                logicPart.Actors[TwoPlayersId.Right] = actorFactory;

            var slardarFactory = ActorFactory.FromRobot(new Player.Slardar(), new SlardarRules());
            List<Frame3D> slardarTrajectory = GetSlardarTrajectory();
            logicPart.NPC.Add(new Tuple<string, ActorFactory, Func<IActor, IController>>(
                SlardarId.LeftTop, slardarFactory, CreateSlardarBot(slardarTrajectory, SlardarId.LeftTop)));
            logicPart.NPC.Add(new Tuple<string, ActorFactory, Func<IActor, IController>>(
                SlardarId.RightBottom, slardarFactory, CreateSlardarBot(slardarTrajectory, SlardarId.RightBottom)));

            logicPart.Bots[PudgeRules.StandingBotName] = () => rules.CreateStandingBot();

            return logicPart;
        }

        private static List<Frame3D> GetSlardarTrajectory()
        {
            return new List<Frame3D>
            {
                new Frame3D(-140, 60, 0),
                new Frame3D(-140, 60, PudgeRules.Current.SlardarWaitDuration),
                new Frame3D(-100, 60, 0),

                new Frame3D(-80, 80, 0),
//                new Frame3D(-110, 110, 0), // Это позволит ему заходить на руну и в нечётные итерации
//                new Frame3D(-110, 110, PudgeRules.Current.SlardarWaitDuration),
                new Frame3D(-80, 80, 0),

                new Frame3D(-60, 100, 0),
                new Frame3D(-60, 140, 0),
                new Frame3D(-60, 140, PudgeRules.Current.SlardarWaitDuration),
                new Frame3D(-60, 100, 0),

                new Frame3D(-80, 80, 0),
                new Frame3D(-120, 120, 0),
                new Frame3D(-120, 120, PudgeRules.Current.SlardarWaitDuration),
                new Frame3D(-80, 80, 0),

                new Frame3D(-100, 60, 0),
            };
        }

        public static Func<IActor, IController> CreateSlardarBot(List<Frame3D> trajectory, string slardarType)
        {
            return actor => new SlardarBot(trajectory, actor, slardarType);
        } 
        static Bot<SlardarCommand> CreateBot()
        {
            return new Bot<SlardarCommand>(turn => turn%5 != 0
                ? new SlardarCommand()
                {
                    GameMovement = new GameMovement()
                    {
                        Angle = 5
                    }
                }
                : new SlardarCommand()
                {
                    GameMovement = new GameMovement()
                    {
                        Range = 10
                    }
                }
                );
        }
    }
}