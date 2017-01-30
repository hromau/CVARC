using AIRLab.Mathematics;
using CVARC.V2;
using System;
using System.Linq;

namespace Pudge.Player
{
    class HookTrigger : Trigger
    {
        private double checkInterval = 0.1; // seconds
        private Hook hook;
        private double startTime;
        private double apogeeTime;
        private double perigeeTime;

        public HookTrigger(Hook hook)
        {
            this.hook = hook;

            startTime = hook.Pudge.World.Clocks.CurrentTime;            
            perigeeTime = startTime + hook.Rules.HookDuration * hook.Pudge.DoubleDamageFactor * 2;
            apogeeTime = startTime + hook.Rules.HookDuration * hook.Pudge.DoubleDamageFactor;

        }

        public override TriggerKeep Act(double time)
        {
            ScheduledTime += checkInterval;

            if (time > apogeeTime && !hook.IsReturning)
            {
                hook.ReturnToStart();
                return TriggerKeep.Keep;
            }

            if (time > perigeeTime && hook.Pudge.World.GetEngine<ICommonEngine>().ContainBody(hook.Id))
            {
                hook.Delete();
                return TriggerKeep.Remove;
            }
            
            if (!hook.IsReturning && CheckCollision())
            {
                hook.ReturnToStart();

                var halfInAirTime = time - startTime;
                hook.Pudge.World.Clocks.AddTrigger(new OneTimeTrigger(time + halfInAirTime, () => hook.Delete()));

                return TriggerKeep.Remove;
            }

            return TriggerKeep.Keep;
        }

        bool CheckCollision()
        {
            var engine = hook.Pudge.World.GetEngine<ICommonEngine>();

            var hookLocation = engine.GetAbsoluteLocation(hook.Id);

            var target = hook.Pudge.World.IdGenerator.GetAllPairsOfType<Units.WADUnit.IWADRobot>()
                .Where(p => p.Item1 != hook.Pudge && engine.ContainBody(p.Item2))
                .Select(p => new { Pair = p, Location = engine.GetAbsoluteLocation(p.Item2) })
                .Where(p => Distance(p.Location, hookLocation) < hook.Rules.HookAttackRadius)
                .Select(p => new { Id = p.Pair.Item2, Robot = p.Pair.Item1 })
                .FirstOrDefault();
            
            if (target != null)
            {
                var scores = target.Robot is PudgeRobot
                    ? PudgeRules.Current.PudgeHookScores
                    : PudgeRules.Current.SlardarHookScores;
                hook.Pudge.World.Scores.Add(hook.Pudge.ControllerId, scores,
                    hook.Pudge.ControllerId + " hooked " + target.Robot.ControllerId);
                target.Robot.Die();
                return true;                
            }
            
            return false;
        }

        private double Distance(Frame3D first, Frame3D second)
        {
            var x = Math.Abs(first.X - second.X);
            var y = Math.Abs(first.Y - second.Y);
            return Math.Sqrt(x*x + y*y);
            //var delta = first.Invert().Apply(second);
            //return Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y + delta.Z * delta.Z);
        }
    }
}
