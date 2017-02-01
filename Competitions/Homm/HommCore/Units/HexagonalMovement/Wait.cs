using AIRLab;
using HoMM.Engine;
using HoMM.Rules;
using System;

namespace HoMM.Units.HexagonalMovement
{
    [Serializable]
    class Wait : IMovement
    {
        public Tuple<Location, double> TryMoveHero(IHommEngine engine, Player player, Map map)
        {
            return Tuple.Create(player.Location, HommRules.Current.WaitDuration);
        }
    }
}
