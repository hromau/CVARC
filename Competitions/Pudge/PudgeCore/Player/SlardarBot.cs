using System;
using System.Collections.Generic;
using System.Linq;
using AIRLab.Mathematics;
using CVARC.V2;
using Pudge.Units.WADUnit;

namespace Pudge.Player
{
    public class SlardarBot : IController
    {
        private Slardar slardar;
        private PathFinder PathFinder;
        private readonly List<Frame3D> Trajectory;
        private readonly string Type;
        private void OnDie()
        {
            PathFinder = new PathFinder(Trajectory, Type);
        }
        public SlardarBot(List<Frame3D> trajectory, IActor slardar, string slardarType)
        {
            this.slardar = Compatibility.Check<Slardar>(this, slardar);
            PathFinder = new PathFinder(trajectory, slardarType);
            Trajectory = trajectory;
            Type = slardarType;
            this.slardar.OnDie += OnDie;
        }

        public void Initialize(IActor controllableActor)
        {
            slardar = Compatibility.Check<Slardar>(this, controllableActor);
        }

        ICommand IController.GetCommand()
        {
            if (slardar.IsDisabled)
                return new SlardarCommand();
            var enemiesDetected = CheckForEnemies();
            if (enemiesDetected)
            {
                slardar.RunAnimation(Animation.Stun);
                return new SlardarCommand {GameMovement = new GameMovement {WaitTime = 1}};
            }
            return PathFinder.GetCommand(slardar.World.GetEngine<ICommonEngine>().GetAbsoluteLocation(slardar.ObjectId));
        }

        private bool CheckForEnemies()
        {
            var enemies = GetEnemiesInScope();
            enemies.ForEach(e => e.Die());
            return enemies.Count > 0;
        }

        public void SendSensorData(object sensorData)
        {
        }

        public double GetAngleToTarget(Frame3D slardarLocation, Frame3D targetLocation)
        {
            var xAxisDeviation = slardarLocation.Yaw.Simplify360().Grad;

            var deltaX = targetLocation.X - slardarLocation.X;
            var deltaY = targetLocation.Y - slardarLocation.Y;

            var angleToTarget = Angle.FromGrad(Math.Atan2(deltaY, deltaX) * 180 / Math.PI).Simplify360().Grad;

            var diff = Math.Abs(angleToTarget - xAxisDeviation);
            var abs = diff > 360 - diff ? 360 - diff : diff;

            return Math.Min(abs, 360 - abs);
        }
        public List<PudgeRobot> GetEnemiesInScope()
        {
            var engine = slardar.World.GetEngine<ICommonEngine>();
            var slardarLocation = engine.GetAbsoluteLocation(slardar.ObjectId);
            return slardar.World.Actors
                .Where(a => a is PudgeRobot)
                .Select(a => Compatibility.Check<PudgeRobot>(this, a))
                .Where(a => a.ObjectId != slardar.ObjectId && !a.IsDisabled)
                .Where(a => !a.IsInvisible)
                .Where(a =>
                {
                    var pudgeLocation = a.World.GetEngine<ICommonEngine>().GetAbsoluteLocation(a.ObjectId);
                    var distance = Geometry.Distance(pudgeLocation.ToPoint3D(), slardarLocation.ToPoint3D());
                    return distance <
                           slardar.Rules.ForwardVisibilityRadius &&
                           GetAngleToTarget(slardarLocation, pudgeLocation) < 45 ||
                           distance < slardar.Rules.SideVisibilityRadius;
                }
                            )
                .Select(a => Compatibility.Check<PudgeRobot>(this, a))
                .ToList();
        }
    }
}