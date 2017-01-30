using System.Collections.Generic;
using CVARC.V2;
using Pudge.Player;
using Pudge.Units.DaggerUnit;
using Pudge.Units.WADUnit;
using Pudge.World;

namespace Pudge
{
    public abstract class PudgeClient<TSensorData> : CvarcClient<TSensorData, PudgeCommand>
        where TSensorData : class
    {
        public const string AssemblyName = "Pudge";
        public abstract string LevelName { get; }

        public TSensorData Configurate(string ip, int port, string cvarcTag, 
            int timeLimit = 90, int operationalTimeLimit = 1000, bool isOnLeftSide = true, int seed = 0, bool speedUp = false)
        {
            var configs = new ConfigurationProposal();
            configs.LoadingData.AssemblyName = AssemblyName;
            configs.LoadingData.Level = LevelName;
            configs.SettingsProposal.SpeedUp = speedUp;
            configs.SettingsProposal.Controllers = new List<ControllerSettings>
            {
                new ControllerSettings
                {
                    ControllerId = isOnLeftSide ? TwoPlayersId.Left : TwoPlayersId.Right,
                    Name = "This",
                    Type = ControllerType.Client
                },
                new ControllerSettings
                {
                    ControllerId = isOnLeftSide ? TwoPlayersId.Right : TwoPlayersId.Left,
                    Name = PudgeRules.StandingBotName,
                    Type = ControllerType.Bot
                }
            };
            configs.SettingsProposal.CvarcTag = cvarcTag;
            configs.SettingsProposal.TimeLimit = timeLimit;
            configs.SettingsProposal.OperationalTimeLimit = operationalTimeLimit;
            return Configurate(port, configs, new PudgeWorldState(seed), ip);
        }

        public TSensorData Move(double distance=10)
        {
            return Act(new PudgeCommand
            {
                GameMovement = new GameMovement
                {
                    Range = distance
                }
            });
        }

        public TSensorData Rotate(double angle)
        {
            return Act(new PudgeCommand
            {
                GameMovement = new GameMovement
                {
                    Angle = angle
                }
            });
        }

        public TSensorData Hook()
        {
            return Act(new PudgeCommand
            {
                MakeHook = true
            });
        }

        public TSensorData Wait(double waitingTime)
        {
            return Act(new PudgeCommand
            {
                GameMovement = new GameMovement
                {
                    WaitTime = waitingTime
                }
            });
        }

        public TSensorData Blink(int x, int y)
        {
            return Act(new PudgeCommand
            {
                DaggerDestination = new DaggerDestinationPoint(x, y),
                MakeDagger = true
            });
        }

        public TSensorData CreateWard()
        {
            return Act(new PudgeCommand
            {
                MakeWard = true
            });
        }
    }

    public class PudgeClientLevel1 : PudgeClient<PudgeSensorsData>
    {
        public override string LevelName
        {
            get { return "Level1"; }
        }
    }

    public class PudgeClientLevel2 : PudgeClient<PudgeSensorsData>
    {
        public override string LevelName
        {
            get { return "Level2"; }
        }
    }

    public class PudgeClientLevel3 : PudgeClient<PudgeSensorsData>
    {
        public override string LevelName
        {
            get { return "Level3"; }
        }
    }
}
