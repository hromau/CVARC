using System;
using System.Collections.Generic;
using System.Linq;
using AIRLab.Mathematics;
using CVARC.V2;
using Pudge.Player;
using Pudge.Units.WADUnit;

namespace Pudge.SlardarAi
{
    public class SlardarBot : IController
    {
        private IActor slardar;
        private readonly PathFinder PathFinder;

        public SlardarBot(List<Frame3D> trajectory, IActor slardar)
        {
            this.slardar = slardar;
            PathFinder = new PathFinder(trajectory);
        }

        public void Initialize(IActor controllableActor)
        {
            slardar = controllableActor;
        }

        ICommand IController.GetCommand()
        {
            return slardar.IsDisabled 
                ? new SlardarCommand { GameMovement = new GameMovement { WaitTime = PudgeRules.Current.WaitDuration } }
                : PathFinder.GetCommand(slardar.World.Engine.GetAbsoluteLocation(slardar.ObjectId));
        }

        public void SendSensorData(object sensorData)
        {
        }
    }
}