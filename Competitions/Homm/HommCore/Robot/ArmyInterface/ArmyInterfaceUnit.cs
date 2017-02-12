using CVARC.V2;
using HoMM.Engine;
using HoMM.Robot;
using HoMM.Rules;
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
            var order = Compatibility.Check<IArmyInterfaceCommand>(this, command).Order;
            if (order == null) return UnitResponse.Denied();

            actor.World.HommEngine.Freeze(actor.ControllerId);
            actor.World.HommEngine.SetAnimation(actor.ControllerId, Animation.Idle);

            order.Apply(actor.Player);

            Debugger.Log($"{actor.ControllerId} resources:");

            foreach (var kv in actor.Player.Resources)
                Debugger.Log($"{kv.Key} - {kv.Value}");

            Debugger.Log($"{actor.ControllerId} army:");
            
            foreach (var kv in actor.Player.Army)
                Debugger.Log($"{kv.Key} - {kv.Value}");

            return UnitResponse.Accepted(HommRules.Current.PurchaseDuration);
        }
    }
}
