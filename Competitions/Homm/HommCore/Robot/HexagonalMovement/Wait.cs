using Infrastructure;
using HoMM.Engine;
using HoMM.Rules;
using System;
using CVARC.V2;
using HoMM.Robot;

namespace HoMM.Units.HexagonalMovement
{
    [Serializable]
    class Wait : IMovement
    {
        private double waitTime;

        public Wait() : this(HommRules.Current.WaitDuration) { }

        public Wait(double time)
        {
            waitTime = time;
        }

        public double Apply(IHommRobot robot)
        {
            var player = robot.Player;
            var engine = robot.World.HommEngine;

            engine.Freeze(player.Name);
            engine.SetPosition(player.Name, player.Location.X, player.Location.Y);

            return waitTime;
        }
    }
}
