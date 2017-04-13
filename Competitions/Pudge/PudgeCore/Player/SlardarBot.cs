using System;
using System.Collections.Generic;
using System.Linq;
using AIRLab.Mathematics;
using CVARC.V2;
using Pudge.Units.WADUnit;
using Infrastructure;

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
            Debugger.Log("Slardar " + slardar.ControllerId + " is searching for enemies");
            var engine = slardar.World.GetEngine<ICommonEngine>();
            var slardarLocation = engine.GetAbsoluteLocation(slardar.ObjectId);
            var actors = slardar.World.Actors
                .Where(a => a is PudgeRobot)
                .Select(a => Compatibility.Check<PudgeRobot>(this, a))
                .ToList();
            var result = new List<PudgeRobot>();
            foreach(var e in actors)
            {
                Debugger.Log("Considering " + e.ObjectId);
                if (e.ObjectId == slardar.ObjectId)
                    Debugger.Log("is Slardar");
                else if (e.IsDisabled)
                    Debugger.Log("is Disabled");
                else if (e.IsInvisible)
                    Debugger.Log("is Invosible");
                else
                {
                    var pudgeLocation = e.World.GetEngine<ICommonEngine>().GetAbsoluteLocation(e.ObjectId);
                    var distance = Geometry.Distance(pudgeLocation.ToPoint3D(), slardarLocation.ToPoint3D());
                    var angle = GetAngleToTarget(slardarLocation, pudgeLocation);
                    Debugger.Log("Target in at distance " + distance + " in angle " + angle);
                    if (angle < 45)
                    {
                        if (distance < slardar.Rules.ForwardVisibilityRadius)
                        {
                            Debugger.Log("Is inside forward visibility "+slardar.Rules.ForwardVisibilityRadius);
                            result.Add(e);
                        }
                        else
                        {
                            Debugger.Log("Is outside forward visibility " + slardar.Rules.ForwardVisibilityRadius);
                        }
                    }
                    else
                    {
                        if (distance < slardar.Rules.SideVisibilityRadius)
                        {
                            Debugger.Log("Is inside side  visibility " + slardar.Rules.SideVisibilityRadius);
                            result.Add(e);
                        }
                        else
                        {
                            Debugger.Log("Is outside side visibility " +slardar.Rules.SideVisibilityRadius);
                            
                        }
                    }
                }
            }
            return result;
        }

        public void SendError(Exception e)
        {
            
        }
    }
}