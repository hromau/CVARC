using System.Collections.Generic;
using CVARC.V2;
using Pudge.Player;
using Pudge.Units.DaggerUnit;
using Pudge.Units.WADUnit;
using Pudge.World;
using Infrastructure;
using System;

namespace Pudge
{
    public class PudgeClient : CvarcClient<PudgeSensorsData, PudgeCommand, PudgeWorldState>
    {
        public const string AssemblyName = "pudge";
        public string LevelName { get; set; }

        public PudgeClient()
        {
            LevelName = "final";
        }
            


        public PudgeSensorsData Configurate(string ip, int port, Guid cvarcTag, 
            int timeLimit = 90, int operationalTimeLimit = 1000, bool isOnLeftSide = true, int seed = 0, bool speedUp = false)
        {
            var configs = new GameSettings();
            configs.LoadingData = new LoadingData();
            configs.LoadingData.AssemblyName = AssemblyName;
            configs.LoadingData.Level = LevelName;
            configs.SpeedUp = speedUp;
            configs.ActorSettings= new List<ActorSettings>
            {
                new ActorSettings
                {
                    ControllerId = isOnLeftSide ? TwoPlayersId.Left : TwoPlayersId.Right,
                    IsBot=false,
                    PlayerSettings=new PlayerSettings
                    {
                        CvarcTag =  cvarcTag
                    }
                },
                new ActorSettings
                {
                    ControllerId = isOnLeftSide ? TwoPlayersId.Right : TwoPlayersId.Left,
                    BotName = PudgeRules.StandingBotName,
                    IsBot=true
                }
            };
            
            configs.TimeLimit = timeLimit;
            configs.OperationalTimeLimit = operationalTimeLimit;
            return Configurate(port, configs, new PudgeWorldState(seed), ip);
        }

        public PudgeSensorsData Move(double distance=10)
        {
            return Act(new PudgeCommand
            {
                GameMovement = new GameMovement
                {
                    Range = distance
                }
            });
        }

        public PudgeSensorsData Rotate(double angle)
        {
            return Act(new PudgeCommand
            {
                GameMovement = new GameMovement
                {
                    Angle = angle
                }
            });
        }

        public PudgeSensorsData Hook()
        {
            return Act(new PudgeCommand
            {
                MakeHook = true
            });
        }

        public PudgeSensorsData Wait(double waitingTime)
        {
            return Act(new PudgeCommand
            {
                GameMovement = new GameMovement
                {
                    WaitTime = waitingTime
                }
            });
        }

        public PudgeSensorsData Blink(int x, int y)
        {
            return Act(new PudgeCommand
            {
                DaggerDestination = new DaggerDestinationPoint(x, y),
                MakeDagger = true
            });
        }

        public PudgeSensorsData CreateWard()
        {
            return Act(new PudgeCommand
            {
                MakeWard = true
            });
        }
    }
    
}
