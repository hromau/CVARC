using CVARC.V2;
using HoMM.Robot;
using HoMM.Robot.HexagonalMovement;
using HoMM.Sensors;
using HoMM.World;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoMM
{
    public class HommClient<TSensorData> : CvarcClient<TSensorData, HommCommand, HommWorldState>
        where TSensorData : class
    {

        public const string AssemblyName = "Homm";
        public string LevelName { get { return "Level1"; } }


        public TSensorData Configurate(string ip, int port, Guid cvarcTag,
            int timeLimit = 90, int operationalTimeLimit = 1000, int seed = 0, bool speedUp = false)
        {
            var configs = new GameSettings();
            configs.LoadingData = new LoadingData();
            configs.LoadingData.AssemblyName = AssemblyName;
            configs.LoadingData.Level = LevelName;
            configs.SpeedUp = speedUp;
            configs.ActorSettings = new List<ActorSettings>
            {
                new ActorSettings
                {
                    ControllerId = TwoPlayersId.Left,
                    IsBot=false,
                    PlayerSettings=new PlayerSettings
                    {
                        CvarcTag =  cvarcTag
                    }
                }
            };

            configs.TimeLimit = timeLimit;
            configs.OperationalTimeLimit = operationalTimeLimit;
            return Configurate(port, configs, new HommWorldState(seed), ip);
        }

        public TSensorData Move(Direction direction)
        {
            return Act(new HommCommand { Movement = new HexMovement(direction) });
        }
    }
}
