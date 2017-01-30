using System;
using System.Collections.Generic;
using AIRLab.Mathematics;
using Pudge.Units.WADUnit;

namespace Pudge.Player
{
    internal class PathFinder
    {
        private const double Epsilon = 2;
        private readonly List<Frame3D> Trajectory;
        private int CurrentPointIndex;
        private readonly string Type;
        public PathFinder(List<Frame3D> trajectory, string slardarType)
        {
            Trajectory = trajectory;
            Type = slardarType;
            CurrentPointIndex = 0;
        }
        public SlardarCommand GetCommand(Frame3D currentLocation)
        {
            var currentTarget = Type == "LeftTop" ? Trajectory[CurrentPointIndex] : Trajectory[CurrentPointIndex] * -1;
            var distance = Geometry.Distance(currentLocation.ToPoint3D(), currentTarget.ToPoint3D());
            if (distance < Epsilon)
            {
                CurrentPointIndex++;
                CurrentPointIndex %= Trajectory.Count;
                currentTarget = Type == "LeftTop" ? Trajectory[CurrentPointIndex] : Trajectory[CurrentPointIndex] * -1;
            }
            if (Trajectory[CurrentPointIndex].Z > 0)
            {
                var waitDuration = Trajectory[CurrentPointIndex].Z;
                CurrentPointIndex++;
                CurrentPointIndex %= Trajectory.Count;
                return new SlardarCommand {GameMovement = new GameMovement {WaitTime = waitDuration}};
            }
            var xAxisDeviation = currentLocation.Yaw.Simplify360().Grad;

            var deltaX = currentTarget.X - currentLocation.X;
            var deltaY = currentTarget.Y - currentLocation.Y;

            var angleToTarget = Angle.FromGrad(Math.Atan2(deltaY, deltaX) * 180 / Math.PI).Simplify360().Grad;

            var diff = Math.Abs(angleToTarget - xAxisDeviation);
            var absDiff = diff > 360 - diff ? 360 - diff : diff;

            var direction = Math.Abs(Angle.FromGrad(xAxisDeviation + absDiff).Simplify360().Grad - angleToTarget) < .1 ||
                Math.Abs(Angle.FromGrad(xAxisDeviation + absDiff).Simplify360().Grad - angleToTarget - 360) < .1
                ? 1
                : -1;


            var isNotToBeRotated = Math.Abs((int)(absDiff / PudgeRules.Current.RotationAngle)) == 0;
            if (isNotToBeRotated)
                return new SlardarCommand
                {
                    GameMovement = new GameMovement { Range = Math.Min(15, distance) }
                };

            return new SlardarCommand
            {
                GameMovement = new GameMovement { Angle = direction * Math.Min(absDiff, 15.0)}
            };
        }
    }
}