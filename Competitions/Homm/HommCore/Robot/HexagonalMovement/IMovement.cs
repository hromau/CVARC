using Infrastructure;
using HoMM.Engine;
using HoMM.Robot;

namespace HoMM.Robot.HexagonalMovement
{
    interface IMovement
    {
        double Apply(IHommRobot robot);
    }
}
