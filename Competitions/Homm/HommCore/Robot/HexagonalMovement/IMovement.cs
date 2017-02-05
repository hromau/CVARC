using Infrastructure;
using HoMM.Engine;
using HoMM.Robot;

namespace HoMM.Units.HexagonalMovement
{
    interface IMovement
    {
        double Apply(IHommRobot robot);
    }
}
