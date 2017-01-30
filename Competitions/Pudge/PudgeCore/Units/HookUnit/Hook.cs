using AIRLab.Mathematics;
using System;
using CVARC.V2;
using Pudge.Units.HookUnit;
using Pudge.World;

namespace Pudge.Player
{
    public class Hook
    {
        public string Id { get; set; }
        public PudgeRobot Pudge { get; private set; }
        public bool IsReturning { get; private set; }
        public bool IsAvailable { get { return !Pudge.IsBuffActivated(PudgeEvent.HookThrown); } }
        public bool IsThrown { get { return Speed != Frame3D.Identity; } }
        public Frame3D StartingLocation { get; private set; }
        public Frame3D Speed { get; private set; }
        public IHookRules Rules { get; private set; }

        public Hook(IHookRobot pudge, IHookRules rules)
        {
            Pudge = Compatibility.Check<PudgeRobot>(this, pudge);
            Speed = new Frame3D();
            StartingLocation = new Frame3D();
            Id = Pudge.World.IdGenerator.CreateNewId(this);
            Rules = rules;
        }
    
        public void Throw()
        {
            IsReturning = false;
            var commonEngine = Pudge.World.GetEngine<ICommonEngine>();
            var pudgeEngine = Pudge.World.GetEngine<IPudgeWorldEngine>();
            StartingLocation = commonEngine.GetAbsoluteLocation(Pudge.ObjectId);
            Pudge.SpawnHook(StartingLocation, Id);
            commonEngine.SetAbsoluteSpeed(Id, Speed = CalculateSpeed(StartingLocation));
            Pudge.World.Clocks.AddTrigger(new HookTrigger(this));
            
            if (Pudge.IsBuffActivated(PudgeEvent.Invisible))
            {
                Pudge.DeleteBuff(PudgeEvent.Invisible);
                pudgeEngine.SetTransparent(Pudge.ObjectId, false);
            }

            pudgeEngine.PlayAnimation(Pudge.ObjectId, Animation.HookStart);
        }

        private Frame3D CalculateSpeed(Frame3D startingLocation)
        {
            var speedX = Math.Cos(startingLocation.Yaw.Radian) * Rules.HookVelocity;
            var speedY = Math.Sin(startingLocation.Yaw.Radian) * Rules.HookVelocity;
            return new Frame3D(speedX, speedY, 0);
        }
        
        public void ReturnToStart()
        {
            IsReturning = true;
            Pudge.World.GetEngine<ICommonEngine>().SetAbsoluteSpeed(Id, Speed = new Frame3D(-Speed.X, -Speed.Y, 0));
        }

        public void Delete()
        {
            var engine = Pudge.World.GetEngine<ICommonEngine>();
            Speed = Frame3D.Identity;
            if (engine.ContainBody(Id))
                engine.DeleteObject(Id);
            Pudge.LastActivatingTime.Remove(PudgeEvent.HookThrown);
        }
    }
}
