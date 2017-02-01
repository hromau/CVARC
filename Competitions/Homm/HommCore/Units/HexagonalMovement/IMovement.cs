using AIRLab;
using HoMM.Engine;

namespace HoMM.Units.HexagonalMovement
{
    interface IMovement
    {
        Tuple<Location, double> TryMoveHero(IHommEngine engine, Player player, Map map);
    }
}
