using CVARC.V2;
using HoMM.ClientClasses;
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
    public class HommClient : CvarcClient<HommSensorData, HommCommand, HommWorldState>
    {
        public const string AssemblyName = "homm";

        public HommSensorData Configurate(string ip, int port, Guid cvarcTag, HommLevel level = HommLevel.Level1, bool isOnLeftSide = true,
            int timeLimit = 90, int operationalTimeLimit = 1000, int seed = 0, bool speedUp = false, bool debugMap = false, bool spectacularView = true)
        {
            var configs = new GameSettings
            {
                LoadingData = new LoadingData
                {
                    AssemblyName = AssemblyName,
                    Level = level.ToString()
                },

                SpectacularView = spectacularView,
                SpeedUp = speedUp,
                TimeLimit = timeLimit,
                OperationalTimeLimit = operationalTimeLimit,

                ActorSettings = new List<ActorSettings>
                {
                    new ActorSettings
                    {
                        ControllerId = isOnLeftSide ? TwoPlayersId.Left : TwoPlayersId.Right,
                        IsBot = false,
                        PlayerSettings = new PlayerSettings
                        {
                            CvarcTag =  cvarcTag
                        }
                    },

                    new ActorSettings
                    {
                        ControllerId = isOnLeftSide ? TwoPlayersId.Right : TwoPlayersId.Left,
                        IsBot = true,
                        BotName = HommRules.StandingBotName
                    }
                }
            };

            return Configurate(port, configs, new HommWorldState(seed, debugMap), ip);
        }

        public HommSensorData Move(Direction movementDirection)
        {
            return Act(new HommCommand { Movement = new HexMovement(movementDirection) });
        }

        public HommSensorData Wait(double waitDurationInSeconds)
        {
            if (waitDurationInSeconds <= 0)
                throw new ArgumentException($"Parameter '{nameof(waitDurationInSeconds)}' should be greater than zero.");

            return Act(new HommCommand { Movement = new HexMovement(waitDurationInSeconds) });
        }

        public HommSensorData HireUnits(int amountOfUnitsToHire)
        {
            if (amountOfUnitsToHire <= 0)
                throw new ArgumentException($"Parameter '{nameof(amountOfUnitsToHire)}' should be greater than zero. Cannot hire a negative amount of units.");

            return Act(new HommCommand { Order = new HireOrder(amountOfUnitsToHire) });
        }

        public HommSensorData BuildGarrison(Dictionary<UnitType, int> armyToLeftInGarrison)
        {
            foreach (var kv in armyToLeftInGarrison)
                if (kv.Value <= 0)
                    throw new ArgumentException($"Units count in garrison should be greater than zero. Cannot create garrison from negative amount of units.");

            return Act(new HommCommand { WaitInGarrison = armyToLeftInGarrison });
        }
    }
}
