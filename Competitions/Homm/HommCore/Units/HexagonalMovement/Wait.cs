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
            engine.Freeze(player.Name);
            engine.SetPosition(player.Name, player.Location.X, player.Location.Y);
            return Tuple.Create(player.Location, HommRules.Current.WaitDuration);
        }
    }
}
