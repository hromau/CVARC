using Infrastructure;
using HoMM.Engine;
using HoMM.Rules;
using System;
using CVARC.V2;

namespace HoMM.Units.HexagonalMovement
{
    [Serializable]
    class Wait : IMovement
    {
        public Tuple<Location, double> TryMoveHero(IHommEngine engine, Player player, Map map)
        {
            Debugger.Settings.EnabledMethod<Wait>(nameof(TryMoveHero));

            Debugger.Log(player.Location);
            Debugger.Log(HommRules.Current);

            return Tuple.Create(player.Location, HommRules.Current.WaitDuration);
        }
    }
}
