using Infrastructure;
using HoMM.Engine;
using HoMM.Robot;

namespace HoMM.Robot.HexagonalMovement
{
    public interface IMovement
    {
        double Apply(IHommRobot robot);
    }
}
