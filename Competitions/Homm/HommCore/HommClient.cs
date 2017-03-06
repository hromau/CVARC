using CVARC.V2;
using HoMM.Robot;
using HoMM.Robot.ArmyInterface;
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
        public const string AssemblyName = "homm";

        public TSensorData Configurate(string ip, int port, Guid cvarcTag, HommLevel level = HommLevel.Level1,
            int timeLimit = 90, int operationalTimeLimit = 1000, int seed = 0, bool speedUp = false, bool debugMap=false, bool spectacularView=true)
        {
            var configs = new GameSettings();
            configs.LoadingData = new LoadingData();
            configs.LoadingData.AssemblyName = AssemblyName;
            configs.LoadingData.Level = level.ToString();
            configs.SpectacularView = spectacularView;
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
            return Configurate(port, configs, new HommWorldState(seed, debugMap), ip);
        }

        public TSensorData Move(Direction movementDirection)
        {
            return Act(new HommCommand { Movement = new HexMovement(movementDirection) });
        }

        public TSensorData Wait(double waitDurationInSeconds)
        {
            if (waitDurationInSeconds <= 0)
                throw new ArgumentException($"Parameter '{nameof(waitDurationInSeconds)}' should be greater than zero.");

            return Act(new HommCommand { Movement = new HexMovement(waitDurationInSeconds) });
        }

        public TSensorData HireUnits(int amountOfUnitsToHire)
        {
            if (amountOfUnitsToHire <= 0)
                throw new ArgumentException($"Parameter '{nameof(amountOfUnitsToHire)}' should be greater than zero. Cannot hire a negative amount of units.");

            return Act(new HommCommand { Order = new HireOrder(amountOfUnitsToHire) });
        }

        public TSensorData BuildGarrison(Dictionary<UnitType, int> armyToLeftInGarrison)
        {
            foreach (var kv in armyToLeftInGarrison)
                if (kv.Value <= 0)
                    throw new ArgumentException($"Units count in garrison should be greater than zero. Cannot create garrison from negative amount of units.");

            return Act(new HommCommand { WaitInGarrison = armyToLeftInGarrison });
        }
    }
}
