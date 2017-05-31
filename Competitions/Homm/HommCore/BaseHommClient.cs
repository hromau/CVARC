using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;
using HoMM.ClientClasses;
using HoMM.Robot;
using HoMM.Robot.ArmyInterface;
using HoMM.Robot.HexagonalMovement;
using HoMM.Robot.ScoutInterface;
using HoMM.World;
using Infrastructure;

namespace HoMM
{
    public abstract class BaseHommClient<TSensorData> : CvarcClient<TSensorData, HommCommand, HommWorldState>
        where TSensorData : class
    {
        private const string AssemblyName = "homm";

        protected abstract HommLevel Level { get; }

        public TSensorData Configurate(string ip, int port, Guid cvarcTag,
            bool isOnLeftSide = true,
            int timeLimit = 90,
            int operationalTimeLimit = 1000,
            int seed = 0,
            bool speedUp = false,
            bool debugMap = false,
            bool spectacularView = true)
        {
            return Configurate(ip, port, cvarcTag, Level, isOnLeftSide, timeLimit, 
                operationalTimeLimit, seed, speedUp, debugMap, spectacularView);
        }

        protected internal TSensorData Configurate(string ip, int port, Guid cvarcTag, 
            HommLevel level = HommLevel.Level1,
            bool isOnLeftSide = true,
            int timeLimit = 90, 
            int operationalTimeLimit = 1000,
            int seed = 0, 
            bool speedUp = false,
            bool debugMap = false, 
            bool spectacularView = true)
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
                            CvarcTag = cvarcTag
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

        public TSensorData Move(Direction movementDirection)
        {
            return Act(new HommCommand {Movement = new HexMovement(movementDirection)});
        }

        public TSensorData Wait(double waitDurationInSeconds)
        {
            if (waitDurationInSeconds <= 0)
                throw new ArgumentException($"Parameter '{nameof(waitDurationInSeconds)}' should be greater than zero.");

            return Act(new HommCommand {Movement = new HexMovement(waitDurationInSeconds)});
        }

        public TSensorData HireUnits(int amountOfUnitsToHire)
        {
            if (amountOfUnitsToHire <= 0)
                throw new ArgumentException($"Parameter '{nameof(amountOfUnitsToHire)}' should be greater than zero. Cannot hire a negative amount of units.");

            return Act(new HommCommand {HireOrder = new HireOrder(amountOfUnitsToHire)});
        }

        public TSensorData BuildGarrison(Dictionary<UnitType, int> armyToLeaveInGarrison)
        {
            foreach (var kv in armyToLeaveInGarrison)
                if (kv.Value <= 0)
                    throw new ArgumentException("Units count in garrison should be greater than zero. Cannot create garrison from negative amount of units.");

            return Act(new HommCommand {WaitInGarrison = armyToLeaveInGarrison});
        }
    }
}