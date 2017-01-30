using System;
using AIRLab.Mathematics;
using CVARC.V2;
using Pudge.Units.PudgeUnit;

namespace Pudge.Units.WADUnit
{
    public class WADUnit : IUnit
    {
        private readonly IWADRobot actor;
        private readonly IWADRules rules;

        public WADUnit(IActor actor)
        {
            this.actor = Compatibility.Check<IWADRobot>(this, actor);
            rules = Compatibility.Check<IWADRules>(this, this.actor.Rules);
        }

        public UnitResponse ProcessCommand(object _command)
        {
            if (actor.IsDisabled) return UnitResponse.Denied();
            var command = Compatibility.Check<IGameCommand>(this, _command).GameMovement;
            if (command == null)
                return UnitResponse.Denied();

            var hasteFactor = actor.HasteFactor;
            var eps = 0.01;
            if (command.Range <= 0 && Math.Abs(command.Angle) < eps && command.WaitTime <= 0)
            {
                Wait();
                return UnitResponse.Accepted(1);
            }
            if (command.WaitTime > eps)
            {
                Wait();
                return UnitResponse.Accepted(command.WaitTime);
            }
            if (command.Range > eps)
            {
                var movementTime = Move(command.Range, hasteFactor);
                return UnitResponse.Accepted(movementTime);
            }
            if (Math.Abs(command.Angle) > eps)
            {
                var rotationTime = Rotate(command.Angle);
                return UnitResponse.Accepted(rotationTime);
            }
            return UnitResponse.Accepted(0.025);
        }

        private double Rotate(double angle)
        {
            var angular = Math.Sign(angle) * rules.RotationVelocity;
            var duration = Math.Abs(angle / angular);

            actor.World.GetEngine<ICommonEngine>().SetRelativeSpeed(actor.ObjectId, Frame3D.Identity.NewYaw(Angle.FromGrad(angular)));
            actor.World.GetEngine<IPudgeWorldEngine>().PlayAnimation(actor.ObjectId, Animation.Walk);
            return duration;
        }

        private void Wait()
        {
            actor.World.GetEngine<IPudgeWorldEngine>().PlayAnimation(actor.ObjectId, Animation.Idle);
            actor.World.GetEngine<ICommonEngine>().SetAbsoluteSpeed(actor.ObjectId, Frame3D.Identity);
        }

        private double Move(double range, double hasteFactor = 1)
        {
            var linear = rules.MovementVelocity * hasteFactor;
            var duration = Math.Abs(range / linear);

            actor.World.GetEngine<ICommonEngine>().SetRelativeSpeed(actor.ObjectId, Frame3D.Identity.NewX(linear));
            actor.World.GetEngine<IPudgeWorldEngine>().PlayAnimation(actor.ObjectId, Animation.Walk);
            return duration;
        }        
    }
}