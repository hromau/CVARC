using AIRLab.Mathematics;
using CVARC.V2;
using Pudge.Player;
using Pudge.Units.PudgeUnit;
using Pudge.World;

namespace Pudge.Units.HookUnit
{
    public class HookUnit : IUnit
    {
        private readonly Hook hook;
        private readonly IHookRobot actor;
        private readonly IHookRules rules;

        public HookUnit(IActor actor)
        {
            this.actor = Compatibility.Check<IHookRobot>(this, actor);
            rules = Compatibility.Check<IHookRules>(this, actor.Rules);
            hook = new Hook(this.actor, rules);            
        }

        public UnitResponse ProcessCommand(object _command)
        {
            if (actor.IsDisabled) return UnitResponse.Denied();

            if (hook.IsThrown)
            {
                hook.Pudge.ActivateBuff(PudgeEvent.HookThrown, hook.Pudge.Rules.HookThrown);
                return UnitResponse.Accepted(0.1);
            }

            var command = Compatibility.Check<IHookCommand>(this, _command);

            if (command.MakeHook && !hook.Pudge.IsBuffActivated(PudgeEvent.HookCooldown))
            {
                if (hook.IsAvailable && actor.World.Configuration.LoadingData.Level != "Level1")
                    Hook();

                return UnitResponse.Accepted(0.1);
            }

            return UnitResponse.Denied();
        }

        private void Hook()
        {
            hook.Pudge.World.GetEngine<ICommonEngine>().SetAbsoluteSpeed(hook.Pudge.ObjectId, Frame3D.Identity);
            hook.Pudge.ActivateBuff(PudgeEvent.HookCooldown, hook.Pudge.Rules.HookCooldown);
            hook.Pudge.ActivateBuff(PudgeEvent.HookThrown, hook.Pudge.Rules.HookThrown);
            hook.Throw();
        }
    }
}