using CVARC.V2;
using HoMM.ClientClasses;
using HoMM.Engine;
using HoMM.Robot;
using Infrastructure;

namespace HoMM.Robot.ArmyInterface
{
    class ArmyInterfaceUnit : IUnit
    {
        private IHommRobot actor;

        public ArmyInterfaceUnit(IHommRobot actor)
        {
            this.actor = actor;
        }

        public UnitResponse ProcessCommand(object command)
        {
            var order = Compatibility.Check<IArmyInterfaceCommand>(this, command).HireOrder;
            if (order == null) return UnitResponse.Denied();

            if (actor.IsDead) return UnitResponse.Accepted(HommRules.Current.UnitsHireDuration);

            actor.World.HommEngine.Freeze(actor.ControllerId);
            actor.World.HommEngine.SetAnimation(actor.ControllerId, Animation.Idle);

            order.Apply(actor.Player);

            Debugger.Log($"{actor.ControllerId} resources:");

            foreach (var kv in actor.Player.Resources)
                Debugger.Log($"{kv.Key} - {kv.Value}");

            Debugger.Log($"{actor.ControllerId} army:");
            
            foreach (var kv in actor.Player.Army)
                Debugger.Log($"{kv.Key} - {kv.Value}");

            return UnitResponse.Accepted(HommRules.Current.UnitsHireDuration);
        }
    }
}
