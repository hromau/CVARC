using AIRLab.Mathematics;
using CVARC.V2;
using System;

namespace Pudge.Units.WADUnit
{
    [Obsolete]
    class MovementTrigger : Trigger
    {
        double interval = 0.5;
        double endTime;
        IWADRobot robot;
        double linear;
        double angular;

        public MovementTrigger(IWADRobot robot, double linear, double angular, double duration)
        {
            this.robot = robot;
            this.linear = linear;
            this.angular = angular;
            endTime = robot.World.Clocks.CurrentTime + duration;
            ScheduledTime = 0;
        }

        public override TriggerKeep Act(double time)
        {
            if (time > endTime) return TriggerKeep.Remove;
            if (robot.IsDisabled) return TriggerKeep.Remove;
            var engine = robot.World.GetEngine<ICommonEngine>();
            var robotAngle = engine.GetAbsoluteLocation(robot.ObjectId).Yaw.Radian;

            var speed = new Frame3D(
                linear * Math.Cos(robotAngle),
                linear * Math.Sin(robotAngle),
                0,
                Angle.Zero,
                Angle.FromGrad(angular),
                Angle.Zero);

            engine.SetAbsoluteSpeed(robot.ObjectId, speed);

            ScheduledTime += interval;
            return TriggerKeep.Keep;
        }
    }
}
