using Infrastructure;
using HoMM.Engine;
using HoMM.Rules;
using System;
using CVARC.V2;
using HoMM.Robot;

namespace HoMM.Robot.HexagonalMovement
{
    [Serializable]
    class Wait : IMovement
    {
        public double WaitTime { get; set; }

        public Wait() : this(HommRules.Current.WaitDuration) { }

        public Wait(double time)
        {
            WaitTime = time;
        }

        public double Apply(IHommRobot robot)
        {
            var player = robot.Player;
            var engine = robot.World.HommEngine;

            engine.Freeze(player.Name);
            engine.SetPosition(player.Name, player.Location.X, player.Location.Y);

            return WaitTime;
        }
    }
}
