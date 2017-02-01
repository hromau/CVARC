using CVARC.V2;
using HoMM.Robot;
using HoMM.Rules;

namespace HoMM.Units.ArmyInterface
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

            order.Apply(actor.Player);

            return UnitResponse.Accepted(HommRules.Current.BuyDuration);
        }
    }
}
