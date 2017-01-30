using System;
using System.Collections.Generic;
using AIRLab.Mathematics;
using CVARC.V2;
using Pudge.Player;
using Pudge.Units.PudgeUnit;
using Pudge.Units.WADUnit;

namespace Pudge.Tests
{
    public class PudgeMovementTests : DefaultPudgeTest
    {
        private const double Eps = .5;
        public readonly LocatorItem CurrentLocation = new LocatorItem { Angle = 45, X = -130, Y = -130 };
        [CvarcTestMethod]
        public void Movement_SimpleMovingForward()
        {
            var newCoordinateX = Robot.Rules.MovementRange * Math.Cos(CurrentLocation.Angle) + CurrentLocation.X;
            var newCoordinateY = Robot.Rules.MovementRange * Math.Sin(CurrentLocation.Angle) + CurrentLocation.Y;
            Robot.GameMove(56);
            AssertEqual(data => data.SelfLocation.X, newCoordinateX, Eps);
            AssertEqual(data => data.SelfLocation.Y, newCoordinateY, Eps);
            AssertTrue(data => Math.Abs(data.SelfLocation.Angle - CurrentLocation.Angle) < Eps);
        }

        [CvarcTestMethod]
        public void Movement_RotateWithHastTest()
        {
            var newAngle = CurrentLocation.Angle - 50;
            Robot.Rotate(-10 * PudgeRules.Current.RotationAngle);
            Robot.GameMove(9 * PudgeRules.Current.MovementRange);
            Robot.Rotate(-360);
            AssertEqual(data => data.SelfLocation.Angle, newAngle, Eps);
        }

        [CvarcTestMethod]
        public void Movement_Rotate()
        {
            Robot.Rotate(360);
            AssertEqual(data => data.SelfLocation.Angle, 45, Eps);
        }
        //[CvarcTestMethod]
        //public void Movement_SimpleRotatingClockwise()
        //{
        //    var newAngle = CurrentLocation.Angle - Robot.Rules.RotationAngle;
        //    Robot.Rotate(WADCommandType.RotateClockwise);
        //    AssertEqual(data => data.SelfLocation.Angle, newAngle, Eps);
        //}

        //[CvarcTestMethod]
        //public void Movement_SimpleRotatingCounterClockwise()
        //{
        //    var newAngle = CurrentLocation.Angle + Robot.Rules.RotationAngle;
        //    Robot.Rotate(WADCommandType.RotateCounterClockwise);
        //    AssertEqual(data => data.SelfLocation.Angle, newAngle, Eps);
        //}


        //[CvarcTestMethod]
        //public void Movement_RotatingClockwiseAndCounterClocwise()
        //{
        //    Robot
        //        .Rotate(WADCommandType.RotateClockwise)
        //        .Rotate(WADCommandType.RotateCounterClockwise);
        //    var newAngle = CurrentLocation.Angle;
        //    AssertEqual(data => data.SelfLocation.Angle, newAngle, Eps);
        //}

        //[CvarcTestMethod]
        //public void Movement_CounterClockwiseReversal()
        //{
        //    Robot.Rotate(WADCommandType.RotateCounterClockwise, (int) (2*Angle.Pi.Grad/Robot.Rules.RotationAngle));
        //    var newAngle = CurrentLocation.Angle;
        //    AssertEqual(data => data.SelfLocation.Angle, newAngle, Eps);
        //}

        //[CvarcTestMethod]
        //public void Movement_ClockwiseReversal()
        //{
        //    Robot.Rotate(WADCommandType.RotateClockwise, (int) (2 * Angle.Pi.Grad / Robot.Rules.RotationAngle));
        //    var newAngle = CurrentLocation.Angle;
        //    AssertEqual(data => data.SelfLocation.Angle, newAngle, Eps);
        //}

        //[CvarcTestMethod]
        //public void Movement_RotateClockwiseAndMove()
        //{
        //    Robot
        //        .Rotate(WADCommandType.RotateClockwise, (int) (45/Robot.Rules.RotationAngle))
        //        .GameMove((int) (Math.Abs(CurrentLocation.Y) / Robot.Rules.MovementRange));
        //    var newCoordinateX = 0;
        //    AssertEqual(data => data.SelfLocation.Y, newCoordinateX, Eps);
        //}
        //[CvarcTestMethod]
        //public void Movement_RotateCounterClockwiseAndMove()
        //{
        //    Robot
        //        .Rotate(WADCommandType.RotateCounterClockwise, (int)(45 / Robot.Rules.RotationAngle))
        //        .GameMove((int)(Math.Abs(CurrentLocation.X) / Robot.Rules.MovementRange));
        //    var newCoordinateY = 0;
        //    AssertEqual(data => data.SelfLocation.Y, newCoordinateY, Eps);
        //}
    }
}